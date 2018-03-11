using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TransmissionRemoteBot.Services.Telegram.Helpers;
using TransmissionRemoteBot.Services.Transmission;

namespace TransmissionRemoteBot.Services.Telegram.Commands
{
    public class StatusCommand : TransmissionCommandBase
    {
        private readonly SelfUpdatingMessage _selfUpdatingMessage;

        public StatusCommand(ITelegramBotClient botClient, ITransmissionService transmissionService,
            ITransmissionConfiguration defaultTransmissionConfiguration, SelfUpdatingMessage selfUpdatingMessage) : base(botClient, transmissionService, defaultTransmissionConfiguration)
        {
            _selfUpdatingMessage = selfUpdatingMessage;
        }

        public override async Task ProcessAsync(Message message, params string[] arguments)
        {
            if (_defaultTransmissionConfiguration == null)
            {
                await _botClient.SendTextMessageAsync(message.Chat.Id, "No servers specified. Add servers with /add command");
                return;
            }

            await _selfUpdatingMessage.Send(message.Chat.Id, async () => {
                var status = await _transmissionService.GetStatusAsync(_defaultTransmissionConfiguration);
                if (status == null)
                {
                    return "Error fetching data";
                }
                return $"Active: {status.ActiveTorrentCount}; Total: {status.torrentCount}; Down speed: {status.downloadSpeed}; Up speed: {status.uploadSpeed}";
            });
        }
    }
}
