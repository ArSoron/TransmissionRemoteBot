﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TransmissionRemoteBot.Services.Transmission
{
    public class TransmissionConfiguration : ITransmissionConfiguration
    {
        public Uri Url { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
