using System;
using System.Linq;
using System.Threading.Tasks;
using GreetingService.Core;
using GreetingService.Core.Entities;
using GreetingService.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function.Invoices
{
    public class ComputeInvoices
    {
        private readonly ILogger _logger;
        private readonly IInvoiceService _invoiceService;
        private readonly IGreetingRepository _greetingRepository;
        private readonly IUserService _userService;

        public ComputeInvoices(ILogger<ComputeInvoices>logger, IGreetingRepository greetingRepository, IUserService userService, IInvoiceService invoiceService)
        {
            _logger = logger;
            _greetingRepository = greetingRepository;
            _userService = userService;
            _invoiceService = invoiceService;
        }

        [FunctionName("ComputeInvoices")]
        //public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        public async Task Run([TimerTrigger("0 2 * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var greetings = await _greetingRepository.GetAsync();

            var greetingsGroupedByInvoice = greetings.GroupBy(x => new { x.From, x.Timestamp.Year, x.Timestamp.Month });

            foreach (var group in greetingsGroupedByInvoice)
            {
                var user = await _userService.GetUserAsync(group.Key.From);
                var invoice = new Invoice
                { 
                    Greetings = group,
                    Month = group.Key.Month,
                    Year = group.Key.Year,
                    Sender = user,
                };

                await _invoiceService.CreateOrUpdateInvoiceAsync(invoice);
            }
        }
    }
}
