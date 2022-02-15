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
    public class ConvertGreetingToXml
    {
        [FunctionName("ConvertGreetingToXml")]
        public async Task Run([BlobTrigger("greetings/{name}", Connection = "LogStorageAccount")]Stream greetingJsonBlob, string name, [Blob("greetings-xml/{name}", FileAccess.Write)] Stream greetingXmlBlob, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {greetingJsonBlob.Length} Bytes");

            var greeting = JsonSerializer.Deserialize<Greeting>(greetingJsonBlob);

            var streamWriter = new StreamWriter(greetingXmlBlob);
            streamWriter.WriteLine("id;from;to;message;timestamp");
            streamWriter.WriteLine($"{greeting.Id};{greeting.From};{greeting.To};{greeting.Message};{greeting.Timestamp}");

            await streamWriter.FlushAsync();
        }
    }
}
