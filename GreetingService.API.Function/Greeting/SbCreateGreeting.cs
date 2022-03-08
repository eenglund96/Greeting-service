using System;
using System.Threading.Tasks;
using GreetingService.API.Core;
using GreetingService.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function
{
    public class SbCreateGreeting
    {
        private readonly ILogger<SbCreateGreeting> _logger;
        private readonly IGreetingRepository _greetingRepository;

        public SbCreateGreeting(ILogger<SbCreateGreeting> log, IGreetingRepository greetingRepository)
        {
            _logger = log;
            _greetingRepository = greetingRepository;
        }

        [FunctionName("SbCreateGreeting")]
        public async Task Run([ServiceBusTrigger("main", "greeting_create", Connection = "ServiceBusConnectionString")] Greeting greeting)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {greeting}");

            try
            {
                await _greetingRepository.CreateAsync(greeting);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to insert Greeting to IGreetingRepository", ex);
                throw;
            }
        }
    }
}
