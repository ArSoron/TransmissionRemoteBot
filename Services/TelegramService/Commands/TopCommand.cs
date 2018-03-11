using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TransmissionRemoteBot.Services.Telegram.Helpers;
using TransmissionRemoteBot.Services.Transmission;

namespace TransmissionRemoteBot.Services.Telegram.Commands
{
    public class TopCommand : TransmissionCommandBase
    {
        private readonly SelfUpdatingMessage _selfUpdatingMessage;

        public TopCommand(ITelegramBotClient botClient, ITransmissionService transmissionService,
            ITransmissionConfiguration defaultTransmissionConfiguration, SelfUpdatingMessage selfUpdatingMessage) : base(botClient, transmissionService, defaultTransmissionConfiguration)
        {
            _selfUpdatingMessage = selfUpdatingMessage;
        }

        public override async Task ProcessAsync(Message message, params string[] arguments)
        {
            int.TryParse(arguments.FirstOrDefault(), out int count);
            if (count == default(int))
                count = 5;
            await ProcessAsync(message, count);
        }

        public async Task ProcessAsync(Message message, int count)
        {
            if (_defaultTransmissionConfiguration == null)
            {
                await _botClient.SendTextMessageAsync(message.Chat.Id, "No servers specified. Add servers with /add command");
                return;
            }

            await _selfUpdatingMessage.Send(message.Chat.Id, async () => {
                var torrents = await _transmissionService.GetTorrentsAsync(_defaultTransmissionConfiguration);
                if (torrents == null)
                {
                    return "Error fetching data";
                }
                return (torrents.Count() > count ? $"Last {count} added torrents\n" : "All torrents\n") +
                string.Join("\n", torrents
                    .OrderByDescending(torrent => torrent.AddedDate)
                    .Take(count)
                    .Select(torrent => $"<strong>{torrent.Name}</strong>\n" +
                    $"\t{torrent.Status.GetDisplayName()} ↓{torrent.RateDownload} ↑{torrent.RateUpload} {torrent.PercentDone * 100}%"));
            });
        }
    }
}
