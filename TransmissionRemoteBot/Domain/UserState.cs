using System.Collections.Generic;

namespace TransmissionRemoteBot.Domain
{
    public class UserState
    {
        public long UserId { get; set; }

        public IEnumerable<TransmissionServer> Servers { get; set; }
    }
}
