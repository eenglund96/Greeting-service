using GreetingService.API.Core;
using GreetingService.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure.GreetingRepository
{
    public class FileGreetingRepository : IGreetingRepository
    {
        private readonly string _filePath = "greeting.json";
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true, };

        public FileGreetingRepository(string filePath)
        {
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
            _filePath = filePath;
        }
        public async Task CreateAsync(Greeting greeting)
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);

            if (greetings.Any(x => x.id == greeting.id))
            {
                throw new Exception($"Greeting with ID: {greeting.id} already exists.");
            }
                
            greetings.Add(greeting);

            File.WriteAllText(_filePath, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
        }

        public async Task <Greeting> GetAsync(Guid id)
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            return greetings?.FirstOrDefault(x => x.id == id);
            throw new Exception($"The {id} does not exist!");
        }

        public async Task <IEnumerable<Greeting>> GetAsync()
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            return greetings;
        }

        public async Task UpdateAsync(Greeting greeting)
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            var existingGreeting = greetings.FirstOrDefault(x => x.id == greeting.id);
            if (existingGreeting == null)
                throw new Exception($"Greeting with id: {greeting.id} not found!");

            existingGreeting.To = greeting.To;
            existingGreeting.From = greeting.From;
            existingGreeting.Message = greeting.Message;

            File.WriteAllText(_filePath, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
        }

        public async Task DeleteAsync(Guid id)
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            var greeting = greetings?.FirstOrDefault(x => x.id == id);
            if (greeting == null)
                throw new Exception($"Greeting with id: {greeting.id} not found!");

            greetings.Remove(greeting);
            File.WriteAllText(_filePath, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
        }

        public async Task DeleteAllAsync()
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            
            if (greetings == null)
                throw new Exception("An exception occured!");

            greetings?.Clear();
            File.WriteAllText(_filePath, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
        }

        public async Task<IEnumerable<Greeting>> GetAsync(string from, string to)
        {
            var greetings = await GetAsync();

            if (!string.IsNullOrEmpty(from))
            {
                greetings = greetings.Where(x => x.From.Equals(from, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(to))
            {
                greetings = greetings.Where(x => x.To.Equals(from, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            else
            {
                throw new Exception("You have encountered an Exception!");
            }

            return greetings;
        }

        public Task DeleteAllAsync(string from, string to)
        {
            throw new NotImplementedException();
        }
    }
}
