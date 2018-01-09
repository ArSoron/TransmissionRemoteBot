using System;

namespace TransmissionRemoteBot.TransmissionService
{
    public interface ITransmissionConfiguration
    {
        Uri Url { get; set; }
        string Login { get; set; }
        string Password { get; set; }
    }
}