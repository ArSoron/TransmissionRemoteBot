using System.Collections;

namespace TransmissionRemoteBot.TransmissionService
{
    public class TransmissionStatus
    {
        public TransmissionStatusArguments Arguments { get; set; }
    }
    public class TransmissionStatusArguments
    {
        public uint ActiveTorrentCount { get; set; }
        public decimal DownloadSpeed { get; set; }
        public uint PausedTorrentCount { get; set; }
        public uint TorrentCount { get; set; }
        public decimal UploadSpeed { get; set; }
    }
}