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

namespace GreetingService.API.Function
{
    public class PostUser
    {
        private readonly ILogger<PostUser> _logger;
        private readonly IAuthHandler _authHandler;
        private readonly IUserService _userService;

        public PostUser(ILogger<PostUser> log, IAuthHandler authHandler, IUserService userService)
        {
            _logger = log;
            _authHandler = authHandler;
            _userService = userService;
        }

        [FunctionName("PostUser")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(User), Description = "The OK response")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Not found")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (!_authHandler.IsAuthorizedAsync(req))
                return new UnauthorizedResult();

            try
            {
                var user = JsonSerializer.Deserialize<User>(req.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                await _userService.CreateUserAsync(user);
                var createdUser = await _userService.GetUserAsync(user.Email);
                return new OkObjectResult(createdUser);
            }

            catch (ArgumentException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            catch
            {
                return new ConflictResult();
            }

        }
    }
}

