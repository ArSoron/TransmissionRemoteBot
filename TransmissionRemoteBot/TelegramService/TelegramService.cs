using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using TransmissionRemoteBot.TransmissionService;

namespace TransmissionRemoteBot.TelegramService
{
    public class TelegramService : ITelegramService
    {
        private readonly ILogger<TelegramService> _logger;
        private readonly ITransmissionConfiguration _defaultTransmissionConfiguration;
        private readonly ITransmissionService _transmissionService;


        private readonly ITelegramBotClient _botClient;

        public TelegramService(ITelegramConfiguration config, ITransmissionService transmissionService, ILoggerFactory loggerFactory, ITransmissionConfiguration defaultTransmissionConfiguration) {
            _logger = loggerFactory.CreateLogger<TelegramService>();
            _defaultTransmissionConfiguration = defaultTransmissionConfiguration;
            _transmissionService = transmissionService;
            _botClient = new TelegramBotClient(config.Apikey);
            _logger.LogInformation("Initialized");
        }

        public void Register()
        {
            using (_logger.BeginScope("Service registration")) {
                _logger.LogInformation("Starting");
                _botClient.OnMessage += BotOnMessageReceived;
                _botClient.OnMessageEdited += BotOnMessageReceived;
                _botClient.OnReceiveError += BotOnReceiveError;

                _botClient.StartReceiving();
                _logger.LogInformation("Now Receiving");
            }
        }

        public void StayingAlive()
        {
            var message = $"I'm alive at {DateTime.UtcNow}";
#if DEBUG
            Debug.Write(message + "DebugLog");
#endif
            _logger.LogInformation(message + "LoggerFactoryLog");
        }

        #region Handlers
        private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage) return;

            switch (message.Text.Split(' ').First())
            {
                case "/start":
                    const string welcomeMessage = @"Hello! Welcome to Transmission Remote Bot!
Choose /add to add your Transmission web interface to bot and start using it.
/help would list all available commands
";
                    await _botClient.SendTextMessageAsync(message.Chat.Id, welcomeMessage);
                    break;
                case "/status":
                    string responseMessage;
                    if (_defaultTransmissionConfiguration == null)
                    {
                        responseMessage = "No servers specified. Add servers with /add command";
                    }
                    else
                    {
                        var status = await _transmissionService.GetStatusAsync(_defaultTransmissionConfiguration);

                        if (status != null)
                        {
                            responseMessage = $"Active: {status.Arguments.ActiveTorrentCount}; Total: {status.Arguments.TorrentCount}; Down speed: {status.Arguments.DownloadSpeed}; Up speed: {status.Arguments.UploadSpeed}";
                        }
                        else
                        {
                            responseMessage = "Error fetching data";
                        }
                    }
                    
                    await _botClient.SendTextMessageAsync(message.Chat.Id, responseMessage);
                    break;
                case "/help":
                default:
                    const string usage = @"Usage:
/add   - add new Transmission Web Interface
";
                    await _botClient.SendTextMessageAsync(
                        message.Chat.Id,
                        usage);
                    break;
            }
        }

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            _logger.LogError($"Received error: {receiveErrorEventArgs.ApiRequestException.ErrorCode} — {receiveErrorEventArgs.ApiRequestException.Message}");
        }
        #endregion

        public void Dispose()
        {
            using (_logger.BeginScope("Service Unregistration"))
            {
                _botClient.StopReceiving();
            }
        }
    }
}
