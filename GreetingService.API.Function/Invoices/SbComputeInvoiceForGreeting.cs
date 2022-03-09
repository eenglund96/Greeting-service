using System;
using System.Linq;
using System.Threading.Tasks;
using GreetingService.API.Core;
using GreetingService.Core;
using GreetingService.Core.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function.Invoices
{
    public class SbComputeInvoiceForGreeting
    {
        private readonly ILogger<SbComputeInvoiceForGreeting> _logger;
        private readonly IGreetingRepository _greetingRepository;
        private readonly IUserService _userService;
        private readonly IInvoiceService _invoiceService;

        public SbComputeInvoiceForGreeting(ILogger<SbComputeInvoiceForGreeting> log, IGreetingRepository greetingRepository, IUserService userService, IInvoiceService invoiceService)
        {
            _logger = log;
            _greetingRepository = greetingRepository;
            _userService = userService;
            _invoiceService = invoiceService;
        }

        [FunctionName("SbComputeInvoiceForGreeting")]
        public async Task Run([ServiceBusTrigger("main", "greeting_compute_invoice", Connection = "ServiceBusConnectionString")]Greeting greeting)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {greeting}");

            try
            {
                var invoice = await _invoiceService.GetInvoiceAsync(greeting.From, greeting.Timestamp.Year, greeting.Timestamp.Month);
                var user = await _userService.GetUserAsync(greeting.From);

                if (invoice == null)
                {
                    try
                    {
                        invoice = new Invoice
                        {
                            Greetings = new[] { greeting },
                            Month = greeting.Timestamp.Month,
                            Year = greeting.Timestamp.Year,
                            Sender = user,
                        };
                        await _invoiceService.CreateOrUpdateInvoiceAsync(invoice);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to create new invoice for greeting {id}", greeting.Id);
                        throw;
                    } 
                }
                else if (!invoice.Greetings.Any(x => x.Id == greeting.Id))
                {
                    try
                    {
                        invoice.Greetings = invoice.Greetings.Append(greeting).ToList();
                        await _invoiceService.CreateOrUpdateInvoiceAsync(invoice);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to update invoice {id} with new Greeting {greetingid}", invoice.Id, greeting.Id);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compute invoice for new greeting {id}}", greeting.Id);
                throw;
            }
        }
    }
}
