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
using System.Reflection.Metadata;
using System.ComponentModel;


namespace GreetingService.Infrastructure.GreetingRepository
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
            var path = $"{greeting.From}/{greeting.To}/{greeting.Id}";
            var blob = _blobContainerClient.GetBlobClient(path);
            if (await blob.ExistsAsync())
                throw new Exception($"Greeting with id: {greeting.Id} already exists!");

            var greetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
            await blob.UploadAsync(greetingBinary);
        }

        public async Task DeleteAllAsync()
        {
            var blobs = _blobContainerClient.GetBlobsAsync();
            await foreach (var blob in blobs)
            {
                var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
                await blobClient.DeleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var blobs = _blobContainerClient.GetBlobsAsync();
            var blob = await blobs.FirstOrDefaultAsync(x => x.Name.EndsWith(id.ToString()));
            if (blob == null)
                throw new Exception($"Greeting with id: {id} could not be found!");

            var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<Greeting> GetAsync(Guid id)
        {
            var blobs = _blobContainerClient.GetBlobsAsync();
            var blob = await blobs.FirstOrDefaultAsync(x => x.Name.EndsWith(id.ToString()));

            if (blob == null)
                throw new Exception($"Greeting with id: {id} not found!");

            var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
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
            var previousGreeting = await GetAsync(greeting.Id);
            var previousGreetingPath = $"{previousGreeting.From}/{previousGreeting.To}/{previousGreeting.Id}";
            var previousGreetingBlobClient = _blobContainerClient.GetBlobClient(previousGreetingPath);
            await previousGreetingBlobClient.DeleteAsync();

            if (!await previousGreetingBlobClient.ExistsAsync())
                throw new Exception("The greeting you searched for could not be found!");

            var newGreetingPath = $"{greeting.From}/{greeting.To}/{greeting.Id}";
            var newGreetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
            var newGreetingBlobClient = _blobContainerClient.GetBlobClient(newGreetingPath);
            await newGreetingBlobClient.UploadAsync(newGreetingBinary);
        }
    }
}
