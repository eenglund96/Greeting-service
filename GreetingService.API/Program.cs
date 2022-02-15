using GreetingService.Core;
using GreetingService.Infrastructure;
using GreetingService.Infrastructure.GreetingRepository;
using GreetingService.Infrastructure.UserService;
using Microsoft.Extensions.Logging.ApplicationInsights;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddXmlSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IGreetingRepository, MemoryGreetingRepository>();

//    (c =>
//{
//var config = c.GetService<IConfiguration>();
//return new FileGreetingRepository(config["FileRepositoryFilePath"]);
//});

builder.Services.AddScoped<IUserService, AppSettingsUserService>();

builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Information);
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPINSIGHTS_CONNECTIONSTRING"]);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
