using GreetingService.API.Core;
using GreetingService.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure.GreetingRepository
{
    public class MemoryGreetingRepository : IGreetingRepository
    {
        private readonly IList<Greeting> _repository = new List<Greeting>();

        public async Task CreateAsync(Greeting greeting)
        {
            _repository.Add(greeting);
        }

        public async Task DeleteAsync(Guid id)
        {
            var greetings = _repository;
            var greeting = greetings?.FirstOrDefault(x => x.Id == id);
            if (greeting == null)
                throw new Exception($"Greeting with id: {greeting.Id} not found!");

            _repository.Remove(greeting);
        }

        public async Task DeleteAllAsync()
        {
            _repository.Clear();
        }

        public async Task <Greeting> GetAsync(Guid id)
        {
            return _repository.FirstOrDefault(x => x.Id == id);
        }

        public async Task <IEnumerable<Greeting>> GetAsync()
        {
            return _repository;
        }

        public async Task UpdateAsync(Greeting greeting)
        {
            var existingGreeting = _repository.FirstOrDefault(x => x.Id == greeting.Id);

            if (existingGreeting == null)
                throw new Exception($"Greeting with id: {greeting.Id} not found!");

            existingGreeting.To = greeting.To;
            existingGreeting.From = greeting.From;
            existingGreeting.Message = greeting.Message;
        }

        public async Task<IEnumerable<Greeting>> GetAsync(string from, string to)
        {
            var greetings = await GetAsync();

            if (!string.IsNullOrEmpty(from))
            {
                greetings = greetings.Where(x => x.From.Equals(from, StringComparison.OrdinalIgnoreCase)).ToList();
            }
               
            else if (!string.IsNullOrEmpty(to))
            {
                greetings = greetings.Where(x => x.To.Equals(from, StringComparison.OrdinalIgnoreCase)).ToList();
            }
         
            return greetings;
        }

        public Task DeleteAllAsync(string from, string to)
        {
            throw new NotImplementedException();
        }
    }
}
