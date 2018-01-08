using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TransmissionRemoteBot.StorageService;

namespace TransmissionRemoteBot.TelegramService
{
    public class TelegramService : ITelegramService
    {
        private readonly ILogger<TelegramService> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly IStorageService _storageService;

        public TelegramService(ITelegramConfiguration config, IStorageService service, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TelegramService>();
            _storageService = service;
            _botClient = new TelegramBotClient(config.Apikey);
            _logger.LogInformation("TelegramServiceInitialized");
        }

        public void Register(CancellationToken token)
        {
            _logger.LogInformation("Starting service registration");
            _botClient.OnMessage += BotOnMessageReceived;
            _botClient.OnMessageEdited += BotOnMessageReceived;
            _botClient.OnCallbackQuery += BotOnCallbackQueryReceived;
            _botClient.OnReceiveError += BotOnReceiveError;

            _botClient.StartReceiving(cancellationToken: token);
            _logger.LogInformation("Now receiving messages");
        }

        public void StayingAlive()
        {
            var message = $"Ah, ha, ha, ha, stayin' alive, stayin' alive at {DateTime.UtcNow}";
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
                    case "/list":
                        await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                        var state = await _storageService.GetUserStateAsync(message.Chat.Id);
                        if (state?.Servers == null)
                        {
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "No servers added yet \n use /add command to add servers");
                        }
                        else
                        {
                            var inlineKeyboard = new InlineKeyboardMarkup(
                                state.Servers.Select(s => new[] {
                                    InlineKeyboardButton.WithCallbackData($"{s.Url}")
                                }).ToArray());
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Following servers available:", replyMarkup: inlineKeyboard);
                        }
                        break;
                    case "/help":
                    default:
                        const string usage = @"Usage:
/add   - add new Transmission Web Interface
/list  - view all added Transmission Web Interfaces
";
                        await _botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            usage);
                        break;
                }
                _logger.LogInformation("Completed");
            }
        }

        private async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            using (_logger.BeginScope("CallbackQuery processing"))
            {
                _logger.LogInformation($"Trying to fetch data from server {callbackQueryEventArgs.CallbackQuery.Data}");
                await _botClient.AnswerCallbackQueryAsync(
                    callbackQueryEventArgs.CallbackQuery.Id,
                    $"Fetching data from {callbackQueryEventArgs.CallbackQuery.Data}");
                //TODO 
                await _botClient.SendTextMessageAsync(callbackQueryEventArgs.CallbackQuery.Message.Chat.Id, $"Error fetching data from {callbackQueryEventArgs.CallbackQuery.Data}", replyMarkup: new ReplyKeyboardRemove());
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
