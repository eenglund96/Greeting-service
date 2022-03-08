using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using GreetingService.API.Core;
using GreetingService.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using GreetingService.API.Function.Authentication;
using System;
using GreetingService.Core.Enum;

namespace GreetingService.API.Function
{
    public class PutGreeting
    {
        private readonly ILogger<PutGreeting> _logger;
        private readonly IMessagingService _messagingService;
        private readonly IAuthHandler _authHandler;

        public PutGreeting(ILogger<PutGreeting> log, IMessagingService messagingService, IAuthHandler authHandler)
        {
            _logger = log;
            _authHandler = authHandler;
            _messagingService = messagingService;
        }

        [FunctionName("PutGreeting")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Greeting" })]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Accepted, Description = "Accepted")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "greeting")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (!await _authHandler.IsAuthorizedAsync(req))
                return new UnauthorizedResult();

            Greeting greeting;
            try
            {
                var body = await req.ReadAsStringAsync();
                greeting = JsonSerializer.Deserialize<Greeting>(body);
            }

            catch (ArgumentException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            try
            {
                await _messagingService.SendAsync(greeting, MessagingServiceSubject.UpdateGreeting);
            }
            catch
            {
                return new NotFoundResult();
            }

            return new AcceptedResult();
        }
    }
}

