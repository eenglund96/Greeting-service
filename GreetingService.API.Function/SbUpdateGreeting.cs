using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function
{
    public class SbUpdateGreeting
    {
        private readonly ILogger<SbUpdateGreeting> _logger;

        public SbUpdateGreeting(ILogger<SbUpdateGreeting> log)
        {
            _logger = log;
        }

        [FunctionName("SbUpdateGreeting")]
        public void Run([ServiceBusTrigger("main", "greeting_update", Connection = "ServiceBusConnectionString")]string mySbMsg)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}
