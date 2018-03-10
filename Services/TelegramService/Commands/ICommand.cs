using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TransmissionRemoteBot.Services.Telegram.Commands
{
    public interface ICommand
    {
        Task ProcessAsync(Message message, params string[] arguments);
    }
}
