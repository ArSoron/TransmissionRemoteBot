namespace TransmissionRemoteBot.Services.Telegram.Commands
{
    public interface ICommandFactory
    {
        ICommand GetCommand(CommandType command);
    }
}
