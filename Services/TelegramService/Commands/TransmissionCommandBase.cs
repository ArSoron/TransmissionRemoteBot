using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TransmissionRemoteBot.Services.Transmission;

namespace TransmissionRemoteBot.Services.Telegram.Commands
{
    public abstract class TransmissionCommandBase: CommandBase
    {
        protected readonly ITransmissionService _transmissionService;
        protected readonly ITransmissionConfiguration _defaultTransmissionConfiguration;

        protected TransmissionCommandBase(ITelegramBotClient botClient, ITransmissionService transmissionService, ITransmissionConfiguration defaultTransmissionConfiguration) : base(botClient)
        {
            _transmissionService = transmissionService;
            _defaultTransmissionConfiguration = defaultTransmissionConfiguration;
        }

        public override abstract Task ProcessAsync(Message message, params string[] arguments);
    }
}
