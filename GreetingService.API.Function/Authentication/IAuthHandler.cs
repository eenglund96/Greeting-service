using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.API.Function.Authentication
{
    public interface IAuthHandler
    {
        public bool IsAuthorized(HttpRequest req);
    }
}
