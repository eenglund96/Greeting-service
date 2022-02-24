using Azure.Storage.Blobs;
using GreetingService.Core;
using GreetingService.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure.UserService
{
    public class BlobUserService : IUserService
    {
        private const string _blobContainerName = "users";
        private readonly BlobContainerClient _blobContainerClient;
        private const string _blobName = "users.json";      
        private readonly ILogger<BlobUserService> _logger;

        public BlobUserService(IConfiguration configuration, ILogger<BlobUserService> logger)
        {
            var connectionString = configuration["LogStorageAccount"];
            _blobContainerClient = new BlobContainerClient(connectionString, _blobContainerName);
            _blobContainerClient.CreateIfNotExists();
            _logger = logger;
        }

        public bool IsValidUser(string username, string password)
        {
            var blob = _blobContainerClient.GetBlobClient(_blobName);

            if (!blob.Exists())
            {
                _logger.LogInformation("Invalid credentials for {username}", username);
                return false;
            }
            var blobContent = blob.DownloadContent();
            var usersDictionary = blobContent.Value.Content.ToObjectFromJson<IDictionary<string, string>>();

            if (usersDictionary.TryGetValue(username, out var storedPassword))
            {
                if (storedPassword == password)
                    return true;
                _logger.LogWarning("Valid credentials for {username}", username);
            }

            return false;
        }

        public async Task <User> GetUserAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task <IEnumerable<User>> GetUsersAsync()
        {
            throw new NotImplementedException();
        }

        public async Task CreateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteUserAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsValidUserAsync(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
