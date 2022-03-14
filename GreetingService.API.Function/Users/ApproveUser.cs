using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using GreetingService.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace GreetingService.API.Function.Users
{
    public class ApproveUser
    {
        private readonly ILogger<ApproveUser> _logger;
        private readonly IUserService _userService;

        public ApproveUser(ILogger<ApproveUser> log, IUserService userService)
        {
            _logger = log;
            _userService = userService;
        }

        [FunctionName("ApproveUser")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Accepted, Description = "The request was accepted!")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Not found")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/approve/{code}")] HttpRequest req, string code)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                await _userService.ApproveUserAsync(code);
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
           
            return new AcceptedResult();
        }
    }
}

