using System;
using System.Security;

namespace TransmissionRemoteBot.Domain
{
    public class TransmissionServer
    {
        public string Url { get; set; }
        public string Login { get; set; }
        public string Password
        {
            get { throw new InvalidOperationException("Use SecurePassword"); }
            set
            {
                SecurePassword = new SecureString();
                foreach (char c in value)
                {
                    SecurePassword.AppendChar(c);
                }
                SecurePassword.MakeReadOnly();
            }
        }
        public SecureString SecurePassword { get; private set; }
    }
}