using System;
using System.Collections.Generic;
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
    public class DeleteGreeting
    {
        private readonly ILogger<DeleteGreeting> _logger;
        private readonly IGreetingRepository _greetingRepository;
        private readonly IAuthHandler _authHandler;

        public DeleteGreeting(ILogger<DeleteGreeting> log, IGreetingRepository greetingRepository, IAuthHandler authHandler)
        {
            _logger = log;
            _greetingRepository = greetingRepository;
            _authHandler = authHandler;
        }

        [FunctionName("DeleteGreeting")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Greeting>), Description = "The OK response")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Not found")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "greeting/{id}")] HttpRequest req, string id)

        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
         
            if (!_authHandler.IsAuthorized(req))
                return new UnauthorizedResult();

            if (!Guid.TryParse(id, out var idGuid))
                return new BadRequestObjectResult($"{id} is not a valid Guid");

            await _greetingRepository.DeleteAsync(idGuid);

         
            return new OkObjectResult($"Deleted greeting with ID: {idGuid}!");

        }
    }
}

