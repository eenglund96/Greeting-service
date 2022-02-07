using GreetingService.API.Core;
using GreetingService.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure
{
    public class MemoryGreetingRepository : IGreetingRepository
    {
        private readonly IList<Greeting> _repository = new List<Greeting>();

        public void Create(Greeting greeting)
        {
            _repository.Add(greeting);
        }

        public void Delete(Guid id)
        {
            var greetings = _repository;
            var greeting = greetings?.FirstOrDefault(x => x.Id == id);
            if (greeting == null)
                throw new Exception($"Greeting with id: {greeting.Id} not found!");

            _repository.Remove(greeting);
        }

        public void DeleteAll()
        {
            _repository.Clear();
        }

        public Greeting Get(Guid id)
        {
            return _repository.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Greeting> Get()
        {
            return _repository;
        }

        public void Update(Greeting greeting)
        {
            var existingGreeting = _repository.FirstOrDefault(x => x.Id == greeting.Id);

            if (existingGreeting == null)
                throw new Exception($"Greeting with id: {greeting.Id} not found!");

            existingGreeting.To = greeting.To;
            existingGreeting.From = greeting.From;
            existingGreeting.Message = greeting.Message;
        }
    }
}
