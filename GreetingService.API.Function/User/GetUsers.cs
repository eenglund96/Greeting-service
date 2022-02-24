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

namespace GreetingService.API.Function
{
    public class GetUsers
    {
        private readonly ILogger<GetUsers> _logger;
        private readonly IAuthHandler _authHandler;
        private readonly IUserService _userService;


        public GetUsers(ILogger<GetUsers> log, IAuthHandler authHandler, IUserService userService)
        {
            _logger = log;
            _authHandler = authHandler;
            _userService = userService;
        }

        [FunctionName("GetUsers")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(User), Description = "The OK response")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Not found")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (!_authHandler.IsAuthorizedAsync(req))
                return new UnauthorizedResult();

            var users = await _userService.GetUsersAsync();

            if (users == null)
                return new NotFoundObjectResult("Not found");

            return new OkObjectResult(users);
        }
    }
}

