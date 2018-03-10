using Telegram.Bot;
using TransmissionRemoteBot.Services.Transmission;

namespace TransmissionRemoteBot.Services.Telegram.Commands
{
    public class CommandFactory : ICommandFactory
    {
        private readonly ITelegramBotClient _botClient;
        private  readonly ITransmissionService _transmissionService;
        private readonly ITransmissionConfiguration _defaultTransmissionConfiguration;
        public CommandFactory(ITelegramBotClient botClient, ITransmissionService transmissionService, ITransmissionConfiguration defaultTransmissionConfiguration)
        {
            _botClient = botClient;
            _transmissionService = transmissionService;
            _defaultTransmissionConfiguration = defaultTransmissionConfiguration;
        }

        public ICommand GetCommand(CommandType command)
        {
            switch (command)
            {
                case CommandType.Start:
                    return new StartCommand(_botClient);
                case CommandType.Status:
                    return new StatusCommand(_botClient, _transmissionService, _defaultTransmissionConfiguration);
                case CommandType.Add:
                    return new AddCommand(_botClient, _transmissionService, _defaultTransmissionConfiguration);
                case CommandType.List:
                    return new ListCommand(_botClient);
                case CommandType.Top:
                    return new TopCommand(_botClient, _transmissionService, _defaultTransmissionConfiguration);
                case CommandType.Help:
                default:
                    return new HelpCommand(_botClient);
            }
        }
    }
}
