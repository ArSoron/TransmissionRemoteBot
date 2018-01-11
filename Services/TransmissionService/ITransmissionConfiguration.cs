using System;

namespace TransmissionRemoteBot.Services.Transmission
{
    public interface ITransmissionConfiguration
    {
        Uri Url { get; set; }
        string Login { get; set; }
        string Password { get; set; }
    }
}