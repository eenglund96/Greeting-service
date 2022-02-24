using System;
using System.IO;
using System.Net;
using System.Text.Json;
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
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GreetingService.API.Function
{
    public class PutUser
    {
        private readonly ILogger<PutUser> _logger;
        private readonly IAuthHandler _authHandler;
        private readonly IUserService _userService;

        public PutUser(ILogger<PutUser> log, IAuthHandler authHandler, IUserService userService)
        {
            _logger = log;
            _authHandler = authHandler;
            _userService = userService;
        }

        [FunctionName("PutUser")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(User), Description = "The OK response")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Not found")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/{email}")] HttpRequest req, string email)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (!_authHandler.IsAuthorizedAsync(req))
                return new UnauthorizedResult();

            try
            {
                var user = JsonSerializer.Deserialize<User>(req.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                await _userService.UpdateUserAsync(user);
                var updatedUser = await _userService.GetUserAsync(user.Email);
                return new OkObjectResult(updatedUser);
            }

            catch (ArgumentException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            catch
            {
                return new NotFoundResult();
            }
        }
    }
}

