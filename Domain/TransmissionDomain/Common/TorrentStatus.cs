using System.ComponentModel.DataAnnotations;

namespace TransmissionRemoteBot.Domain.Transmission.Common
{
    public enum TorrentStatus
    {
        [Display(Name = "Stopped")]
        TR_STATUS_STOPPED = 0, /* Torrent is stopped */
        [Display(Name = "Check queued")]
        TR_STATUS_CHECK_WAIT = 1, /* Queued to check files */
        [Display(Name = "Checking files")]
        TR_STATUS_CHECK = 2, /* Checking files */
        [Display(Name = "Queued to download")]
        TR_STATUS_DOWNLOAD_WAIT = 3, /* Queued to download */
        [Display(Name = "Downloading")]
        TR_STATUS_DOWNLOAD = 4, /* Downloading */
        [Display(Name = "Queued to seed")]
        TR_STATUS_SEED_WAIT = 5, /* Queued to seed */
        [Display(Name = "Seeding")]
        TR_STATUS_SEED = 6  /* Seeding */
    }
}
