using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using GreetingService.API.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function
{
    public class ConvertGreetingToCsv
    {
        [FunctionName("ConvertGreetingToCsv")]
        public async Task Run([BlobTrigger("greetings/{name}", Connection = "LogStorageAccount")]Stream greetingJsonBlob, string name, [Blob("greetings-csv/{name}", FileAccess.Write, Connection = "LogStorageAccount")] Stream greetingCsvBlob, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {greetingJsonBlob.Length} Bytes");

            var greeting = JsonSerializer.Deserialize<Greeting>(greetingJsonBlob);

            var streamWriter = new StreamWriter(greetingCsvBlob);
            streamWriter.WriteLine("id;from;to;message;timestamp");
            streamWriter.WriteLine($"{greeting.id};{greeting.From};{greeting.To};{greeting.Message};{greeting.Timestamp}");

            await streamWriter.FlushAsync();

        }
    }
}
