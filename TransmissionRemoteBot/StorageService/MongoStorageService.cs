using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using TransmissionRemoteBot.Domain;

namespace TransmissionRemoteBot.StorageService
{
    public class MongoStorageService : IStorageService
    {
        private readonly ILogger _logger;
        private readonly IMongoClient client;
        public MongoStorageService(IMongoStorageConfiguration config, ILoggerFactory loggerFactory) {
            _logger = loggerFactory.CreateLogger<MongoStorageService>();
            client = new MongoClient(config.ConnectionString);
        }
        public async Task<UserState> GetUserStateAsync(long userId)
        {
            using (_logger.BeginScope("GetUserStateFromMongo"))
            {
                try
                {
                    var database = client.GetDatabase("TransmissionRemoteBot_dev");
                    var collection = database.GetCollection<UserState>("UserState");
                    var filter = Builders<UserState>.Filter.Eq("UserId", userId);
                    var projection = Builders<UserState>.Projection.Exclude("_id");
                    return await collection.Find(filter).Project<UserState>(projection).FirstOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Exception reading value for user {userId}");
                }
                return null;
            }
        }
    }
}
