using GreetingService.API.Core;
using GreetingService.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure
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
        public void Create(Greeting greeting)
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);

            if (greetings.Any(x => x.Id == greeting.Id))
            {
                throw new Exception($"Greeting with ID: {greeting.Id} already exists.");
            }
                
            greetings.Add(greeting);

            File.WriteAllText(_filePath, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
        }

        public Greeting Get(Guid id)
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            return greetings?.FirstOrDefault(x => x.Id == id);
            throw new Exception($"The {id} does not exist!");
        }

        public IEnumerable<Greeting> Get()
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            return greetings;
        }

        public void Update(Greeting greeting)
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            var existingGreeting = greetings.FirstOrDefault(x => x.Id == greeting.Id);
            if (existingGreeting == null)
                throw new Exception($"Greeting with id: {greeting.Id} not found!");

            existingGreeting.To = greeting.To;
            existingGreeting.From = greeting.From;
            existingGreeting.Message = greeting.Message;

            File.WriteAllText(_filePath, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
        }

        public void Delete(Guid id)
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            var greeting = greetings?.FirstOrDefault(x => x.Id == id);
            if (greeting == null)
                throw new Exception($"Greeting with id: {greeting.Id} not found!");

            greetings.Remove(greeting);
            File.WriteAllText(_filePath, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
        }

        public void DeleteAll()
        {
            var content = File.ReadAllText(_filePath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            
            if (greetings == null)
                throw new Exception("An exception occured!");

            greetings?.Clear();
            File.WriteAllText(_filePath, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
        }
    }
}
