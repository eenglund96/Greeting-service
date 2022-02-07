using GreetingService.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.API.Function.Authentication
{
    public class BasicAuthHandler : IAuthHandler
    {
        private readonly IUserService _userService;

        public BasicAuthHandler(IUserService userService)
        {
            _userService = userService;
        }
        public bool IsAuthorized(HttpRequest req)
        {
            try
            {
                string authHeader = req.Headers["Authorization"];
                if (!string.IsNullOrWhiteSpace(authHeader))
                {
                    var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
                    if (authHeaderValue.Scheme.Equals(AuthenticationSchemes.Basic.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        var credentials = Encoding.UTF8
                                            .GetString(Convert.FromBase64String(authHeaderValue.Parameter ?? string.Empty))
                                            .Split(':', 2);
                        if (credentials.Length == 2)
                        {
                            if (_userService.IsValidUser(credentials[0], credentials[1]))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
