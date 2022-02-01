using Microsoft.AspNetCore.Mvc;

namespace GreetingService.API.Authentication
{
    public class BasicAuthAttribute : TypeFilterAttribute
    {
        public BasicAuthAttribute(string realm = "Basic") : base(typeof(BasicAuthFilter))
        {
            Arguments = new object[] { realm };
        }
    }
}
