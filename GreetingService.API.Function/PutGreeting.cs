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

namespace GreetingService.API.Function
{
    public class PutGreeting
    {
        private readonly ILogger<PutGreeting> _logger;
        private readonly IGreetingRepository _greetingRepository;
        private readonly IAuthHandler _authHandler;

        public PutGreeting(ILogger<PutGreeting> log, IGreetingRepository greetingRepository, IAuthHandler authHandler)
        {
            _logger = log;
            _greetingRepository = greetingRepository;
            _authHandler = authHandler;
        }

        [FunctionName("PutGreeting")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Greeting" })]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Accepted, Description = "Accepted")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "greeting")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (!_authHandler.IsAuthorized(req))
                return new UnauthorizedResult();

            var body = await req.ReadAsStringAsync();
            var greeting = JsonSerializer.Deserialize<Greeting>(body);

            try
            {
                _greetingRepository.Update(greeting);
            }

            catch
            {
                return new NotFoundResult();
            }
     
            return new AcceptedResult();
        }
    }
}

