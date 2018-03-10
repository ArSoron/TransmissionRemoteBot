using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TransmissionRemoteBot.Services.Transmission;

namespace TransmissionRemoteBot.Services.Telegram.Commands
{
    public class AddCommand : TransmissionCommandBase
    {
        public AddCommand(ITelegramBotClient botClient, ITransmissionService transmissionService,
            ITransmissionConfiguration defaultTransmissionConfiguration) : base(botClient, transmissionService, defaultTransmissionConfiguration)
        {
        }

        public override async Task ProcessAsync(Message message, params string[] arguments)
        {
            await ProcessAsync(message, arguments.FirstOrDefault());
        }

        private async Task ProcessAsync(Message message, string torrentLink)
        {
            string statusText = "";
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
        }
    }
}
