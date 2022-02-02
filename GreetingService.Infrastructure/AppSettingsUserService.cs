using GreetingService.Core;
using Microsoft.Extensions.Configuration;
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

        public AppSettingsUserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsValidUser(string username, string password)
        {
            var storedPassword = _configuration[username];
            if (storedPassword != null && password == storedPassword)
            {
               return true;
            }

            return false;
        }
    }
}
