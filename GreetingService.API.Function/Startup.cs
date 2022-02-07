using GreetingService.API.Function.Authentication;
using GreetingService.Core;
using GreetingService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


[assembly: FunctionsStartup(typeof(GreetingService.API.Function.Startup))]
namespace GreetingService.API.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddLogging();
            builder.Services.AddSingleton<IGreetingRepository, MemoryGreetingRepository>();
                
            //    (c =>
            //{
            //    var config = c.GetService<IConfiguration>();
            //    return new FileGreetingRepository(config["FileRepositoryFilePath"]);
            //});

            builder.Services.AddScoped<IUserService, AppSettingsUserService>();
            builder.Services.AddScoped<IAuthHandler, BasicAuthHandler>();
        }
    }  
}

