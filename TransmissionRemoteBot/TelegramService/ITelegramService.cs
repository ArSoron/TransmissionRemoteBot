using System;
using System.Threading;

namespace TransmissionRemoteBot.TelegramService
{
    public interface ITelegramService : IDisposable
    {
        void StayingAlive();
        void Register(CancellationToken token = default(CancellationToken));
    }
}
