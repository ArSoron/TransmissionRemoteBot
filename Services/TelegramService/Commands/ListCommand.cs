using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TransmissionRemoteBot.Services.Telegram.Commands
{
    public class ListCommand : CommandBase
    {
        public ListCommand(ITelegramBotClient botClient) : base(botClient)
        {
        }

        public override async Task ProcessAsync(Message message, params string[] arguments)
        {
            await _botClient.SendTextMessageAsync(
            message.Chat.Id,
            "All commands: \n/" + string.Join("\n/", Enum.GetNames(typeof(CommandType))));
        }
    }
}
