using GreetingService.API.Core;
using GreetingService.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure.GreetingRepository
{
    public class CosmosGreetingRepository : IGreetingRepository
    {
        private readonly ILogger _logger;
        private const string _cosmosDatabaseName = "greetingsdb";
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly PartitionKey _partitionKey = new PartitionKey("/id");

        public CosmosGreetingRepository(IConfiguration configuration, ILogger <CosmosGreetingRepository>logger, CosmosClient cosmosClient)
        {
            var connectionString = configuration["CosmosDbConnectionString"];
            _cosmosClient = new CosmosClient(connectionString, _cosmosDatabaseName);
            _cosmosClient.CreateDatabaseIfNotExistsAsync(_cosmosDatabaseName);
            _logger = logger;
        }

        public async Task CreateAsync(Greeting greeting)
        {
            await _container.UpsertItemAsync(greeting);
            throw new NotImplementedException();
        }

        public async Task DeleteAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAllAsync(string from, string to)
        {
            
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Guid id)
        {
            var cosmosItem = _container.GetItemLinqQueryable<Greeting>().Where(x => x.Id == id);
            //await _container.DeleteItemAsync<Greeting>(_partitionKey);
 

             throw new NotImplementedException();
        }

        public async Task<Greeting> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Greeting>> GetAsync()
        {
            List<Greeting> greetingList = new();
            var iterator = _container.GetItemLinqQueryable<Greeting>().ToFeedIterator();

            while (iterator.HasMoreResults)
            {
                foreach (var cosmosItem in await iterator.ReadNextAsync())
                {
                    greetingList.Add(cosmosItem);
                }
            }
            return greetingList;
        }

        public Task<IEnumerable<Greeting>> GetAsync(string from, string to)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Greeting greeting)
        {
            await _container.UpsertItemAsync(greeting);
        }
    }
}
