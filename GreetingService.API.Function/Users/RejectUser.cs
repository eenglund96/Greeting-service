using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
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

namespace GreetingService.API.Function.Users
{
    public class RejectUser
    {
        private readonly ILogger<RejectUser> _logger;
        private readonly IUserService _userService;

        public RejectUser(ILogger<RejectUser> log, IUserService userService)
        {
            _logger = log;
            _userService = userService;
        }

        [FunctionName("RejectUser")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Accepted, Description = "Request Accepted!")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Not found")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/reject/{code}")] HttpRequest req, string code)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                await _userService.RejectUserAsync(code);
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }

            return new AcceptedResult();
        }
    }
}

