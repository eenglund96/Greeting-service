using GreetingService.API.Core;
using GreetingService.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.EntityFrameworkCore;
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
        private const string _cosmosContainerName = "greetings";

        public CosmosGreetingRepository(ILogger <CosmosGreetingRepository>logger, CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(_cosmosDatabaseName, _cosmosContainerName);
            _logger = logger;
        }

        public async Task CreateAsync(Greeting greeting)
        {
            await _container.UpsertItemAsync(greeting, new PartitionKey(greeting.id.ToString()));
        }

        public async Task DeleteAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAllAsync(string from, string to)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Guid Id)
        {
            await _container.DeleteItemAsync<Greeting>(Id.ToString(), new PartitionKey(Id.ToString()));
        }

        public async Task<Greeting> GetAsync(Guid id)
        {
            var cosmosItem = await _container.GetItemLinqQueryable<Greeting>().FirstOrDefaultAsync(x => x.id == id);
            if (cosmosItem == null)
                throw new Exception("Not found!");

            return cosmosItem;

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

        public async Task<IEnumerable<Greeting>> GetAsync(string from, string to)
        {
            var q = "SELECT * FROM c WHERE 1 = 1";
            var qFrom = $" AND c['From'] = '{from}'";
            var qTo = $" AND c.To = '{to}'";

            List<Greeting> greetingList = new();

            if (!string.IsNullOrEmpty(from))
            {
                q += qFrom;
            }

            if (!string.IsNullOrEmpty(to))
            {
                q += qTo;
            }

            QueryDefinition query = new QueryDefinition(q);

            var iterator = _container.GetItemQueryIterator<Greeting>(query);

            while (iterator.HasMoreResults)
            {
                var results = await iterator.ReadNextAsync();
                greetingList.AddRange(results.ToList());
            }

            return greetingList;

        }

        public async Task UpdateAsync(Greeting greeting)
        {
            await _container.UpsertItemAsync(greeting);
        }
    }
}
