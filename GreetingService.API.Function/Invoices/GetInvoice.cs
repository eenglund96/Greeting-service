using System.IO;
using System.Net;
using System.Threading.Tasks;
using GreetingService.API.Function.Authentication;
using GreetingService.Core;
using GreetingService.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace GreetingService.API.Function.Invoices
{
    public class GetInvoice
    {
        private readonly ILogger<GetInvoice> _logger;
        private readonly IAuthHandler _authHandler;
        private readonly IInvoiceService _invoiceService;

        public GetInvoice(ILogger<GetInvoice> log, IAuthHandler authHandler, IInvoiceService invoiceService)
        {
            _logger = log;
            _authHandler = authHandler;
            _invoiceService = invoiceService;
        }

        [FunctionName("GetInvoice")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Invoice" })]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Accepted, Description = "Accepted")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "invoice/{email}/{year}/{month}")] HttpRequest req, string email, int year, int month)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (!await _authHandler.IsAuthorizedAsync(req))
                return new UnauthorizedResult();

            if (!EmailValidator.IsValid(email))
                return new BadRequestObjectResult($"{email} is not a valid email address");

            var invoices = await _invoiceService.GetInvoiceAsync(email, year, month);
            return new OkObjectResult(invoices);
        }
    }
}

