using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TransmissionRemoteBot.Services.Telegram.Commands
{
    public class StartCommand : CommandBase
    {
        const string welcomeMessage = @"Hello! Welcome to Transmission Remote Bot!
Choose /add to add your Transmission web interface to bot and start using it.
/help would list all available commands
";

        public StartCommand(ITelegramBotClient botClient) : base(botClient)
        {
        }

        public override async Task ProcessAsync(Message message, params string[] arguments)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, welcomeMessage);
        }
    }
}
