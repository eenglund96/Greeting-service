using Azure.Storage.Blobs;
using GreetingService.API.Core;
using GreetingService.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure
{
    public class BlobGreetingRepository : IGreetingRepository
    {
        private const string _blobContainerName = "greetings";
        private readonly BlobContainerClient _blobContainerClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };
        public BlobGreetingRepository(IConfiguration configuration)
        {
            var connectionString = configuration["LogStorageAccount"];
            _blobContainerClient = new BlobContainerClient(connectionString, _blobContainerName);
            _blobContainerClient.CreateIfNotExists();
        }

        public async Task CreateAsync(Greeting greeting)
        {
            var blob = _blobContainerClient.GetBlobClient(greeting.Id.ToString());
            if (await blob.ExistsAsync())
                throw new Exception($"Greeting with id: {greeting.Id} already exists!");

            var greetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
            await blob.UploadAsync(greetingBinary);
        }

        public Task DeleteAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Greeting> GetAsync(Guid id)
        {
            var blobClient = _blobContainerClient.GetBlobClient(id.ToString());
            if (!await blobClient.ExistsAsync())
                throw new Exception($"Greeting with id: {id} not found!");

            var blobContent = await blobClient.DownloadContentAsync();
            var greeting = blobContent.Value.Content.ToObjectFromJson<Greeting>();
            return greeting;
        }

        public async Task<IEnumerable<Greeting>> GetAsync()
        {
            var greetings = new List<Greeting>();
            var blobs = _blobContainerClient.GetBlobsAsync();
            await foreach (var blob in blobs)
            {
                var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
                var blobContent = await blobClient.DownloadContentAsync();
                var greeting = blobContent.Value.Content.ToObjectFromJson<Greeting>();
                greetings.Add(greeting);
            }

            return greetings;
        }

        public async Task UpdateAsync(Greeting greeting)
        {
            var blobClient = _blobContainerClient.GetBlobClient(greeting.Id.ToString());
            if (!await blobClient.ExistsAsync())
                throw new Exception("The greeting you searched for could not be found!");

            await blobClient.DeleteIfExistsAsync();
            var greetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
            await blobClient.UploadAsync(greetingBinary);
        }
    }
}
