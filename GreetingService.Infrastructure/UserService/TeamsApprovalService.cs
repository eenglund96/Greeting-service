using GreetingService.Core;
using GreetingService.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure.UserService
{
    public class TeamsApprovalService : IApprovalService
    {
        private readonly HttpClient _httpClient;
        private readonly string _teamsWebHookUrl;
        private readonly string _greetingServiceBaseUrl;
        private readonly ILogger<TeamsApprovalService> _logger;
        public TeamsApprovalService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<TeamsApprovalService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _teamsWebHookUrl = configuration["TeamsWebHookUrl"];
            _greetingServiceBaseUrl = configuration["GreetingServiceBaseUrl"];
            _logger = logger;
        }

        public async Task BeginUserApprovalAsync(User user)
        {

            var json = @$"{{
						""@type"": ""MessageCard"",
						""@context"": ""https://schema.org/extensions"",
						""summary"": ""Approval for new GreetingService user"",
						""sections"": [
							{{
									""title"": ""**Pending approval for admin Emelie Englund**"",
								""activityImage"": ""https://upload.wikimedia.org/wikipedia/commons/5/51/Mr._Smiley_Face.svg"",
								""activityTitle"": ""Approve new user in GreetingService: {user.Email}"",
								""activitySubtitle"": ""{user.FirstName} {user.LastName}"",
								""facts"": [
									{{
										""name"": ""Date submitted:"",
										""value"": ""{DateTime.Now:yyyy-MM-dd HH:mm}""
									}},
									{{
										""name"": ""Details:"",
										""value"": ""Please approve or reject the new user with email: {user.Email} for the GreetingService""
									}}
								]
							}},
							{{
								""potentialAction"": [
									{{
										""@type"": ""HttpPOST"",
										""name"": ""Approve"",
										""target"": ""{_greetingServiceBaseUrl}/api/user/approve/{user.ApprovalCode}""
	
									}},
									{{
										""@type"": ""HttpPOST"",
										""name"": ""Reject"",
										""target"": ""{_greetingServiceBaseUrl}/api/user/reject/{user.ApprovalCode}""
									}}
								]
							}}
						]
					}}";

            // Perform Connector POST operation     
            var response = await _httpClient.PostAsync(_teamsWebHookUrl, new StringContent(json));
            // Read response content
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content?.ReadAsStringAsync();
                _logger.LogError("Failed to send approval to Microsoft Teams for user {email}. Received this response message: {response}", user.Email, responseContent ?? "null");
            }

			response.EnsureSuccessStatusCode();
        }
    }
}
