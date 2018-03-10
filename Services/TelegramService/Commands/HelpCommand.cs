using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TransmissionRemoteBot.Services.Telegram.Commands
{
    public class HelpCommand : CommandBase
    {
        const string usage = @"Usage:
/add   - add new Transmission Web Interface
";


        public HelpCommand(ITelegramBotClient botClient) : base(botClient)
        {
        }

        public override async Task ProcessAsync(Message message, params string[] arguments)
        {
            await _botClient.SendTextMessageAsync(
            message.Chat.Id,
            usage);
        }
    }
}
