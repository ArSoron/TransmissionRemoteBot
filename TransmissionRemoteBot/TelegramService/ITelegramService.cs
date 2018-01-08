using System;

namespace TransmissionRemoteBot.TelegramService
{
    public interface ITelegramService : IDisposable
    {
        void StayingAlive();
        void Register();
    }
}
