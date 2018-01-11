using System;

namespace TransmissionRemoteBot.Services.Telegram
{
    public interface ITelegramService : IDisposable
    {
        void StayingAlive();
        void Register();
    }
}
