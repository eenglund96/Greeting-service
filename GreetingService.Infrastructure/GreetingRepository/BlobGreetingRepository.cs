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
        private const string _blobCsvContainerName = "greetings-csv";
        private readonly BlobContainerClient _blobContainerClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };
        private readonly string _connectionString;
        public BlobGreetingRepository(IConfiguration configuration)
        {
            _connectionString = configuration["LogStorageAccount"];
            _blobContainerClient = new BlobContainerClient(_connectionString, _blobContainerName);
            _blobContainerClient.CreateIfNotExists();
        }

        public async Task CreateAsync(Greeting greeting)
        {
            var path = $"{greeting.From}/{greeting.To}/{greeting.id}";
            var blob = _blobContainerClient.GetBlobClient(path);
            if (await blob.ExistsAsync())
                throw new Exception($"Greeting with id: {greeting.id} already exists!");

            var greetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
            await blob.UploadAsync(greetingBinary);
        }

        public async Task DeleteAllAsync()
        {
            await DeleteAllAsync(_blobContainerName);
            await DeleteAllAsync(_blobCsvContainerName);
            //await DeleteAllAsync(_blobContainerName);
            //await DeleteAllAsync(_blobCsvContainerName);
        }

        private async Task DeleteAllAsync(string containerName)
        {
            var blobContainerClient = new BlobContainerClient(_connectionString, containerName);
            var blobs = blobContainerClient.GetBlobsAsync();
            await foreach (var blob in blobs)
            {
                var blobClient = blobContainerClient.GetBlobClient(blob.Name);
                await blobClient.DeleteAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            await DeleteAsync(id, _blobContainerName);
            await DeleteAsync(id, _blobCsvContainerName);
        }

        private async Task DeleteAsync(Guid id, string containerName)
        {
            var blobContainerClient = new BlobContainerClient(_connectionString, containerName);
            var blobs = blobContainerClient.GetBlobsAsync();
            var blob = await blobs.FirstOrDefaultAsync(x => x.Name.EndsWith(id.ToString()));
            if (blob == null)
                throw new Exception($"Greeting with id: {id} could not be found!");

            var blobClient = blobContainerClient.GetBlobClient(blob.Name); //_blobContainerClient
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
            var previousGreeting = await GetAsync(greeting.id);
            var previousGreetingPath = $"{previousGreeting.From}/{previousGreeting.To}/{previousGreeting.id}";
            var previousGreetingBlobClient = _blobContainerClient.GetBlobClient(previousGreetingPath);

            if (!await previousGreetingBlobClient.ExistsAsync())
                throw new Exception("The greeting you searched for could not be found!");

            await previousGreetingBlobClient.DeleteAsync();

            var newGreetingPath = $"{greeting.From}/{greeting.To}/{greeting.id}";
            var newGreetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
            var newGreetingBlobClient = _blobContainerClient.GetBlobClient(newGreetingPath);
            await newGreetingBlobClient.UploadAsync(newGreetingBinary);
        }

        public async Task<IEnumerable<Greeting>> GetAsync(string from, string to)
        {
            var prefix = "";
            if (!string.IsNullOrEmpty(from))
            {
                prefix = from;
                if (!string.IsNullOrEmpty(to))
                {
                    prefix = $"{prefix}/{to}";
                }
            }

            var blobs = _blobContainerClient.GetBlobsAsync(prefix: prefix);

            var greetings = new List<Greeting>();
            await foreach (var blob in blobs)
            {
                var blobNameParts = blob.Name.Split('/');

                if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to) && blob.Name.StartsWith($"{from}/{to}/"))
                {
                    Greeting greeting = await DownloadBlob(blob);
                    greetings.Add(greeting);
                }
                else if (!string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to) && blob.Name.StartsWith($"{from}/"))
                {
                    Greeting greeting = await DownloadBlob(blob);
                    greetings.Add(greeting);
                }
                else if (string.IsNullOrEmpty(from) && !string.IsNullOrWhiteSpace(to) && blobNameParts[1].Equals(to))
                {
                    Greeting greeting = await DownloadBlob(blob);
                    greetings.Add(greeting);
                }
                else if (string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to))
                {
                    Greeting greeting = await DownloadBlob(blob);
                    greetings.Add(greeting);
                }
            }

            return greetings;
        }

        private async Task<Greeting> DownloadBlob(Azure.Storage.Blobs.Models.BlobItem blob)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
            var blobContent = await blobClient.DownloadContentAsync();
            var greeting = blobContent.Value.Content.ToObjectFromJson<Greeting>();

            return greeting;
        }

        public async Task DeleteAllAsync(string from, string to)
        {
            var prefix = "";
            if (!string.IsNullOrEmpty(from))
            {
                prefix = from;
                if (!string.IsNullOrEmpty(to))
                {
                    prefix = $"{prefix}/{to}";
                }
            }

            var blobs = _blobContainerClient.GetBlobsAsync(prefix: prefix);
     

            await foreach (var blob in blobs)
            {
                var blobNameParts = blob.Name.Split('/');

                if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to) && blob.Name.StartsWith($"{from}/{to}/"))
                {
                    var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
                    await blobClient.DeleteAsync();
                }
                else if (!string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to) && blob.Name.StartsWith($"{from}/"))
                {
                    var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
                    await blobClient.DeleteAsync();
                }
                else if (string.IsNullOrEmpty(from) && !string.IsNullOrWhiteSpace(to) && blobNameParts[1].Equals(to))
                {
                    var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
                    await blobClient.DeleteAsync();
                }
                else if (string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to))
                {
                    var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
                    await blobClient.DeleteAsync();
                }
            }
        }
    }
}
