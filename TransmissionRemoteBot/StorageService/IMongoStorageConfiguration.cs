namespace TransmissionRemoteBot.StorageService
{
    public interface IMongoStorageConfiguration
    {
        string ConnectionString { get; }
    }
}