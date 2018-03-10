using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TransmissionRemoteBot.Services.Transmission;

namespace TransmissionRemoteBot.Services.Telegram.Commands
{
    public class StatusCommand : TransmissionCommandBase
    {
        public StatusCommand(ITelegramBotClient botClient, ITransmissionService transmissionService,
            ITransmissionConfiguration defaultTransmissionConfiguration) : base(botClient, transmissionService, defaultTransmissionConfiguration)
        {
        }

        public override async Task ProcessAsync(Message message, params string[] arguments)
        {
            string statusText = "";
            if (_defaultTransmissionConfiguration == null)
            {
                await _botClient.SendTextMessageAsync(message.Chat.Id, "No servers specified. Add servers with /add command");
                return;
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
        }
    }
}
