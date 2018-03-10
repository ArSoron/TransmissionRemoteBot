using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TransmissionRemoteBot.Services.Transmission;

namespace TransmissionRemoteBot.Services.Telegram.Commands
{
    public class TopCommand : TransmissionCommandBase
    {
        public TopCommand(ITelegramBotClient botClient, ITransmissionService transmissionService,
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
            var torrents = await _transmissionService.GetTorrentsAsync(_defaultTransmissionConfiguration);
            if (torrents == null)
            {
                await _botClient.SendTextMessageAsync(message.Chat.Id, "Error fetching data");
                return;
            }
            statusText = "Last 5 added torrents: \n" + string.Join("\n", torrents
                .OrderByDescending(torrent => torrent.AddedDate)
                .Take(5)
                .Select(torrent => $"{torrent.Name} ⬇️{torrent.RateDownload} ⬆️{torrent.RateUpload}, ETA {torrent.ETA}"));
            await _botClient.SendTextMessageAsync(message.Chat.Id, statusText);
            return;
        }
    }
}
