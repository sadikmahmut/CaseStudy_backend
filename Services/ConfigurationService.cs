using backend_app.Helpers;
using backend_app.Model;
using backend_app.ViewModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend_app.Services
{
    public class ConfigurationService
    {
        private readonly IMongoCollection<Configuration> _configurations;

        public ConfigurationService(IOptions<MongoDbSettings> options)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionString);
            _configurations = mongoClient.GetDatabase(options.Value.DatabaseName)
                .GetCollection<Configuration>(options.Value.CollectionName);
        }

        public async Task<List<Configuration>> Get(Guid userId) =>
            await _configurations.Find(c => c.UserId == userId).ToListAsync();

        /*public async Task<Configuration> Get(Guid id) =>
            await _configurations.Find(c => c.Id == id).FirstOrDefaultAsync();*/

        public async Task Create(CreateConfigurationVM newConfiguration) 
        {
            Configuration c = new Configuration
            {
                UserId = newConfiguration.UserId,
                BuildingType = newConfiguration.BuildingType,
                BuildingCost = newConfiguration.BuildingCost,
                ConstructionTime = newConfiguration.ConstructionTime
            };

            await _configurations.InsertOneAsync(c);
        }

        public async Task Update(Guid id, Configuration updateConfiguration) =>
            await _configurations.ReplaceOneAsync(c => c.Id == id, updateConfiguration);

        public async Task Remove(Guid id) =>
            await _configurations.DeleteOneAsync(c => c.Id == id);
    }
}
