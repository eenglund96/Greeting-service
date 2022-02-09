using GreetingService.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure
{
    public class AppSettingsUserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AppSettingsUserService> _logger;

        public AppSettingsUserService(IConfiguration configuration, ILogger<AppSettingsUserService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public bool IsValidUser(string username, string password)
        {
            var storedPassword = _configuration[username];
            if (storedPassword != null && password == storedPassword)
            {
                _logger.LogInformation("Valid credentials for {username}", username);
               return true;
            }

            _logger.LogWarning("Invalid credentials for {username}", username);
            return false;
        }
        //Adding a little comment. 
    }
}
