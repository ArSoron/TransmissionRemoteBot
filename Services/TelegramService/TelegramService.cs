﻿using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TransmissionRemoteBot.Services.Transmission;

namespace TransmissionRemoteBot.Services.Telegram
{
    public class TelegramService : ITelegramService
    {
        private readonly ILogger<TelegramService> _logger;
        private readonly ITransmissionConfiguration _defaultTransmissionConfiguration;
        private readonly ITransmissionService _transmissionService;


        private readonly ITelegramBotClient _botClient;

        public TelegramService(ITelegramConfiguration config, ITransmissionService transmissionService, ILoggerFactory loggerFactory, ITransmissionConfiguration defaultTransmissionConfiguration)
        {
            _logger = loggerFactory.CreateLogger<TelegramService>();
            _defaultTransmissionConfiguration = defaultTransmissionConfiguration;
            _transmissionService = transmissionService;
            _botClient = new TelegramBotClient(config.Apikey);
            _logger.LogInformation("Initialized");
        }

        public void Register()
        {
            using (_logger.BeginScope("Service registration"))
            {
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
            Debug.Write(message + " DebugLog");
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
                    string statusText = "";
                    if (_defaultTransmissionConfiguration == null)
                    {
                        await _botClient.SendTextMessageAsync(message.Chat.Id, "No servers specified. Add servers with /add command");
                        break;
                    }                    

                    var cancellationTokenSource = new CancellationTokenSource();
                    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
                    Message sentMessage = null;
                    await Repeat.Interval(TimeSpan.FromSeconds(1), async () =>
                     {
                         var status = await _transmissionService.GetStatusAsync(_defaultTransmissionConfiguration);
                         if (status == null)
                         {
                             await _botClient.SendTextMessageAsync(message.Chat.Id, "Error fetching data");
                             return;
                         }
                         var oldMessage = statusText;
                         statusText = $"Active: {status.ActiveTorrentCount}; Total: {status.torrentCount}; Down speed: {status.downloadSpeed}; Up speed: {status.uploadSpeed}";
                         if (statusText != oldMessage)
                         {
                             sentMessage = sentMessage?.MessageId == null 
                             ? await _botClient.SendTextMessageAsync(message.Chat.Id, "🔄 " + statusText)
                             : await _botClient.EditMessageTextAsync(message.Chat.Id, sentMessage.MessageId, "🔄 " + statusText);
                         }
                     }, cancellationTokenSource.Token);
                    await _botClient.EditMessageTextAsync(message.Chat.Id, sentMessage.MessageId, statusText);
                    break;
                case "/add":
                    var torrentLink = message.Text.Split(' ').Skip(1).FirstOrDefault();
                    if (!Uri.TryCreate(torrentLink, UriKind.Absolute, out Uri torrentUri))
                    {
                        statusText = "Wrong uri";
                    }
                    else
                    {
                        var newTorrent = await _transmissionService.AddTorrentAsync(_defaultTransmissionConfiguration, torrentUri);
                        if (newTorrent != null)
                        {
                            statusText = $"Torrent added: {newTorrent.Name}";
                        }
                        else
                        {
                            statusText = "Error adding torrent";
                        }
                    }
                    await _botClient.SendTextMessageAsync(message.Chat.Id, statusText);
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
