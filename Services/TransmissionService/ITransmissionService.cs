using System;
using System.Threading.Tasks;
using TransmissionRemoteBot.Domain.Transmission.Entity;

namespace TransmissionRemoteBot.Services.Transmission
{
    public interface ITransmissionService
    {
        Task<Statistic> GetStatusAsync(ITransmissionConfiguration config);
        Task<TorrentInfoBase> AddTorrentAsync(ITransmissionConfiguration config, Uri uri);
    }
}