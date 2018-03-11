using Telegram.Bot;
using TransmissionRemoteBot.Services.Telegram.Helpers;
using TransmissionRemoteBot.Services.Transmission;

namespace TransmissionRemoteBot.Services.Telegram.Commands
{
    public class CommandFactory : ICommandFactory
    {
        private readonly ITelegramBotClient _botClient;
        private  readonly ITransmissionService _transmissionService;
        private readonly ITransmissionConfiguration _defaultTransmissionConfiguration;
        private readonly SelfUpdatingMessage _selfUpdatingMessage;

        public CommandFactory(ITelegramBotClient botClient, ITransmissionService transmissionService,
            ITransmissionConfiguration defaultTransmissionConfiguration, SelfUpdatingMessage selfUpdatingMessage)
        {
            _botClient = botClient;
            _transmissionService = transmissionService;
            _defaultTransmissionConfiguration = defaultTransmissionConfiguration;
            _selfUpdatingMessage = selfUpdatingMessage;
        }

        public ICommand GetCommand(CommandType command)
        {
            switch (command)
            {
                case CommandType.Start:
                    return new StartCommand(_botClient);
                case CommandType.Status:
                    return new StatusCommand(_botClient, _transmissionService, _defaultTransmissionConfiguration, _selfUpdatingMessage);
                case CommandType.Add:
                    return new AddCommand(_botClient, _transmissionService, _defaultTransmissionConfiguration);
                case CommandType.List:
                    return new ListCommand(_botClient);
                case CommandType.Top:
                    return new TopCommand(_botClient, _transmissionService, _defaultTransmissionConfiguration, _selfUpdatingMessage);
                case CommandType.Help:
                default:
                    return new HelpCommand(_botClient);
            }
        }
    }
}
