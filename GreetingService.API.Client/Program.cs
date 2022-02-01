using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using System.Threading.Tasks;

namespace GreetingService.API.Client
{
    public class GreetingServiceClient
    {
        private static HttpClient _httpClient = new();

        private const string _getGreetingsCommand = "get greetings";
        private const string _getGreetingCommand = "get greeting";
        private const string _postGreetingCommand = "post greeting";
        private const string _updateGreetingCommand = "update greeting";
        private const string _deleteGreetingCommand = "delete greeting";
        private const string _deleteGreetingsCommand = "delete all";
        private const string _exportGreetingscommand = "export greetings";
        private const string _repeatingCallsCommand = "repeat calls ";
        private static string _from = "Emelie";
        private static string _to = "Elias";

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to command line Greeting client!");
            Console.WriteLine("Please enter the name of the greeting sender:");
            var from = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(from))
                _from = from;

            Console.WriteLine("Enter name of greeting recipient:");
            var to = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(to))
                _to = to;

            while (true)
            {
                Console.WriteLine("Available commands:");
                Console.WriteLine(_getGreetingsCommand);
                Console.WriteLine($"{_getGreetingCommand} [id]");
                Console.WriteLine($"{_postGreetingCommand} [message]");
                Console.WriteLine($"{_updateGreetingCommand} [id] [message]");
                Console.WriteLine($"{_deleteGreetingCommand} [id]");
                Console.WriteLine(_deleteGreetingsCommand);
                Console.WriteLine(_exportGreetingscommand);
                Console.WriteLine($"{_repeatingCallsCommand} [count]");

                Console.WriteLine("Write your command and press [enter] to execute!");

                var command = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(command))
                {
                    Console.WriteLine("Command cannot be empty!");
                    continue;
                }

                if (command.Equals(_getGreetingsCommand, StringComparison.OrdinalIgnoreCase))
                {
                    await GetGreetingsAsync();
                }
                else if (command.StartsWith(_getGreetingCommand, StringComparison.OrdinalIgnoreCase))
                {
                    var idPart = command.Replace(_getGreetingCommand, "");
                    if (Guid.TryParse(idPart, out var id))
                    {
                        await GetGreetingAsync(id);
                    }
                    else
                    {
                        Console.WriteLine($"{idPart} is not a valid Guid!");
                    }
                }
                else if (command.StartsWith(_postGreetingCommand, StringComparison.OrdinalIgnoreCase))
                {
                    var message = command.Replace(_postGreetingCommand, "");
                    await PostGreetingAsync(message);
                }
                else if (command.StartsWith(_updateGreetingCommand, StringComparison.OrdinalIgnoreCase))
                {
                    var idAndMessagePart = command.Replace(_updateGreetingCommand, "") ?? "";
                    var idPart = idAndMessagePart.Trim().Split(" ").First();
                    var messagePart = idAndMessagePart.Replace(idPart, "").Trim();

                    if (Guid.TryParse(idPart, out var id))
                    {
                        await UpdateGreetingAsync(id, messagePart);
                    }
                    else
                    {
                        Console.WriteLine($"{idPart} is not a valid GUID!");
                    }
                }

                else if (command.StartsWith(_deleteGreetingCommand, StringComparison.OrdinalIgnoreCase))
                {
                    var idPart = command.Replace(_deleteGreetingCommand, "").Trim();

                    if (Guid.TryParse(idPart, out var id))
                    {
                        await DeleteGreetingsAsync(id);
                    }
                    else
                    {
                        Console.WriteLine($"{idPart} is not a valid GUID!");
                    }
                }

                else if (command.StartsWith(_deleteGreetingsCommand, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Are you sure you want to delete all of your greetings, yes/no? ");
                    var answer = Console.ReadLine();

                    var answer1 = "yes";
                    var answer2 = "no";

                    if (answer.Equals(answer1))
                    {
                        await DeleteGreetings();
                    }

                    else if (answer.Equals(answer2))
                    {
                        Console.WriteLine("You have aborted the mission! Please choose another command.");
                    }
                }

                else if (command.Equals(_exportGreetingscommand, StringComparison.OrdinalIgnoreCase))
                {
                    await ExportGreetingsAsync();
                }

                else if (command.StartsWith(_repeatingCallsCommand))
                {
                    var countPart = command.Replace(_repeatingCallsCommand, "");

                    if (int.TryParse(countPart, out var count))
                    {
                        await RepeatCallsAsync(count);
                    }
                    else
                    {
                        Console.WriteLine($"Could not parse {countPart} as int");
                    }
                }

                else
                {
                    Console.WriteLine("Command not recognized!");
                }
            }
        }

        private static async Task<IEnumerable<Greeting>> GetGreetingsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5002/api/Greeting");
                response.EnsureSuccessStatusCode();                                                 //throws exception if HTTP response status is not a success status
                var responseBody = await response.Content.ReadAsStringAsync();

                //Do something with response
                var greetings = JsonSerializer.Deserialize<IEnumerable<Greeting>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                foreach (var greeting in greetings)
                {
                    Console.WriteLine($"[{greeting.Id}] [{greeting.Timestamp}] ({greeting.From} -> {greeting.To}) - {greeting.Message}");
                }

                Console.WriteLine();
                return greetings;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Get greetings failed: {e.Message}\n");
            }

            return Enumerable.Empty<Greeting>();
        }

        private static async Task GetGreetingAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:5002/api/Greeting/{id}");
                response.EnsureSuccessStatusCode();                                                 //throws exception if HTTP response status is not a success status
                var responseBody = await response.Content.ReadAsStringAsync();

                //Do something with response
                var greeting = JsonSerializer.Deserialize<Greeting>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Console.WriteLine($"[{greeting.Id}] [{greeting.Timestamp}] ({greeting.From} -> {greeting.To}) - {greeting.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Get greeting failed {e.Message}\n");
            }
        }

        private static async Task PostGreetingAsync(string message)
        {
            try
            {
                var greeting = new Greeting
                {
                    From = _from,
                    To = _to,
                    Message = message,
                    Timestamp = DateTime.Now,
                };
                var response = await _httpClient.PostAsJsonAsync("http://localhost:5002/api/Greeting", greeting);
                Console.WriteLine($"Posted greeting. Service responded with: {response.StatusCode}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Write greeting failed: {e.Message}\n");
            }
        }

        private static async Task UpdateGreetingAsync(Guid id, string message)
        {
            try
            {
                var greeting = new Greeting
                {
                    Id = id,
                    From = _from,
                    To = _to,
                    Message = message,
                };
                var response = await _httpClient.PutAsJsonAsync($"http://localhost:5002/api/Greeting/{id}", greeting);
                Console.WriteLine($"Updated greeting. Service responded with: {response.StatusCode}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Update greeting failed: {e.Message}\n");
            }
        }

        private static async Task DeleteGreetingsAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"http://localhost:5002/api/Greeting/{id}");
                response.EnsureSuccessStatusCode();
                Console.WriteLine($"Deleted id: {id}!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Delete method failed: {e.Message}\n");
            }
        }

        private static async Task DeleteGreetings()
        {
            var response = await _httpClient.GetAsync("http://localhost:5002/api/Greeting");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (greetings.Count == 0)
            {
                Console.WriteLine("There are no greetings in your list!");
            }
            else
            {
                foreach (var greeting in greetings)
                {
                    var id = greeting.Id;
                    await _httpClient.DeleteAsync($"http://localhost:5002/api/Greeting/{id}");
                }
                Console.WriteLine("You have now deleted all of your greetings!");
            }
        }

        private static async Task ExportGreetingsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5002/api/Greeting/");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var greetings = JsonSerializer.Deserialize<List<Greeting>>(responseBody);

                var filename = "ExportedGreetings.xml";
                var xmlWriterSettings = new XmlWriterSettings
                {
                    Indent = true,
                };

                using var xmlWriter = XmlWriter.Create(filename, xmlWriterSettings);
                var serializer = new XmlSerializer(typeof(List<Greeting>));
                serializer.Serialize(xmlWriter, greetings);

                Console.WriteLine($"Exported {greetings.Count()} greetings to {filename}!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to export greetings. Reason: {e}.");
            }
        }

        private static async Task RepeatCallsAsync(int count)
        {
            var greetings = await GetGreetingsAsync();
            var greeting = greetings.Last();

            //init a jobs list
            var random = new Random();
            var jobs = new List<int>();
            for (int i = 1; i <= count; i++)
            {
                jobs.Add(i);
            }

            var stopwatch = Stopwatch.StartNew();           //use stopwatch to measure elapsed time just like a real world stopwatch

            //I cheat by running multiple calls in parallel for maximum throughput - we will be limited by our cpu, wifi, internet speeds
            //This is a bit advanced and the syntax is new with lamdas - don't worry if you don't understand all of it.
            //I always copy this from the internet and adapt to my needs
            //Running this in Visual Studio debugger is slow, try running .exe file directly from File Explorer or command line prompt
            await Parallel.ForEachAsync(jobs, new ParallelOptions { MaxDegreeOfParallelism = 50 }, async (job, token) =>
            {
                var start = stopwatch.ElapsedMilliseconds;
                var response = await _httpClient.GetAsync("https://emelie-appservice-dev.azurewebsites.net/api/Greeting/cbaddfee-5498-47c7-8723-a5ca3c83a55b");
                var end = stopwatch.ElapsedMilliseconds;

                Console.WriteLine($"Response: {response.StatusCode} - Call: {job} - latency: {end - start} ms - rate/s: {job / stopwatch.Elapsed.TotalSeconds} - Message: {greeting.Message}");
            });
        }
    }
}
