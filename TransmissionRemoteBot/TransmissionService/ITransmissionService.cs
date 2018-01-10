using System.Threading.Tasks;
using TransmissionRemoteBot.Domain.Entity;

namespace TransmissionRemoteBot.TransmissionService
{
    public interface ITransmissionService
    {
        Task<Statistic> GetStatusAsync(ITransmissionConfiguration config);
    }
}
