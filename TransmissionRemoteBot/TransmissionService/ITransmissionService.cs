using System.Threading.Tasks;

namespace TransmissionRemoteBot.TransmissionService
{
    public interface ITransmissionService
    {
        Task<TransmissionStatus> GetStatusAsync(ITransmissionConfiguration config);
    }
}
