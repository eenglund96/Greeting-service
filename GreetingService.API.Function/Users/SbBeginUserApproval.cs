using System;
using System.Threading.Tasks;
using GreetingService.Core;
using GreetingService.Core.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function.Users
{
    public class SbBeginUserApproval
    {
        private readonly ILogger<SbBeginUserApproval> _logger;
        private readonly IApprovalService _approvalService;

        public SbBeginUserApproval(ILogger<SbBeginUserApproval> log, IApprovalService approvalService)
        {
            _logger = log;
            _approvalService = approvalService;
        }

        [FunctionName("SbBeginUserApproval")]
        public async Task Run([ServiceBusTrigger("main", "user_approval", Connection = "ServiceBusConnectionString")]User user)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {user}");

            try
            {
                await _approvalService.BeginUserApprovalAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to begin user approval", ex);
                throw;
            }
        }
    }
}
