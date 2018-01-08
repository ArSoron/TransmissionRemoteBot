using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace TransmissionRemoteBot.TelegramService
{
    public class TelegramService : ITelegramService
    {
        private readonly ILogger<TelegramService> _logger;
        private readonly ITelegramBotClient _botClient;

        public TelegramService(ITelegramConfiguration config, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TelegramService>();
            _botClient = new TelegramBotClient(config.Apikey);
            _logger.LogInformation("Initialized");
        }

        public void Register(CancellationToken token)
        {
            _logger.LogInformation("Starting service registration");
            _botClient.OnMessage += BotOnMessageReceived;
            _botClient.OnMessageEdited += BotOnMessageReceived;
            _botClient.OnReceiveError += BotOnReceiveError;

            _botClient.StartReceiving(cancellationToken: token);
            _logger.LogInformation("Now receiving messages");
        }

        public void StayingAlive()
        {
            var message = $"I'm alive at {DateTime.UtcNow}";
            _logger.LogInformation(message);
        }

        #region Handlers
        private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            using (_logger.BeginScope("Message processing"))
            {
                _logger.LogInformation("Started");
                var message = messageEventArgs.Message;

                if (message == null || message.Type != MessageType.TextMessage)
                {
                    _logger.LogWarning("Wrong message type");
                    return;
                }

                _logger.LogInformation($"Started; Message: {message.Text}");

                switch (message.Text.Split(' ').First())
                {
                    case "/start":
                        const string welcomeMessage = @"Hello! Welcome to Transmission Remote Bot!
Choose /add to add your Transmission web interface to bot and start using it.
/help would list all available commands
";
                        await _botClient.SendTextMessageAsync(message.Chat.Id, welcomeMessage);
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
                _logger.LogInformation("Completed");
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
