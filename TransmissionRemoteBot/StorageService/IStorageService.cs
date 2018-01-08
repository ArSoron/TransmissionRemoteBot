using System.Threading.Tasks;
using TransmissionRemoteBot.Domain;

namespace TransmissionRemoteBot.StorageService
{
    public interface IStorageService
    {
        Task<UserState> GetUserStateAsync(long userId);
    }
}
