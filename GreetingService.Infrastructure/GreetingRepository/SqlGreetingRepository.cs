using GreetingService.API.Core;
using GreetingService.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure.GreetingRepository
{
    public class SqlGreetingRepository : IGreetingRepository
    {
        private readonly GreetingDbContext _greetingDbContext;
        public SqlGreetingRepository(GreetingDbContext greetingDbContext)
        {
            _greetingDbContext = greetingDbContext;
        }

        public async Task CreateAsync(Greeting greeting)
        {
            await _greetingDbContext.Greetings.AddAsync(greeting);
            await _greetingDbContext.SaveChangesAsync();
        }

        public async Task DeleteAllAsync()
        {
           var deleteGreetings = await _greetingDbContext.Greetings.ToListAsync();
           //await _greetingDbContext.Remove(deleteGreetings);
           await _greetingDbContext.SaveChangesAsync();
        }

        public Task DeleteAllAsync(string from, string to)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Guid id)
        {
            var deletegreeting = await _greetingDbContext.Greetings.FirstOrDefaultAsync(x => x.Id == id);

            _greetingDbContext.Greetings.Remove(deletegreeting);

            _greetingDbContext.SaveChanges();
        }

        public async Task<Greeting> GetAsync(Guid id)
        {
           var greeting = await _greetingDbContext.Greetings.FirstOrDefaultAsync(x => x.Id == id);
            if (greeting == null)
                throw new Exception("Not found!");

            return greeting;
        }

        public async Task<IEnumerable<Greeting>> GetAsync()
        {
           return await _greetingDbContext.Greetings.ToListAsync();
        }

        public async Task<IEnumerable<Greeting>> GetAsync(string from, string to)
        {
            if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
            {
                var greetings = _greetingDbContext.Greetings.Where(x => x.From.Equals(from) && x.To.Equals(to));
                return await greetings.ToListAsync();
            }
           
            else if (!string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to))
            {
                var greetings = _greetingDbContext.Greetings.Where(x => x.From.Equals(from));
                return await greetings.ToListAsync();
            }
           
            else if (string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
            {
                var greetings = _greetingDbContext.Greetings.Where(x => x.To.Equals(to));
                return await greetings.ToListAsync();
            }

            return await _greetingDbContext.Greetings.ToListAsync();
        }

        public async Task UpdateAsync(Greeting greeting)
        {
            var existingGreeting = await _greetingDbContext.Greetings.FirstOrDefaultAsync(x => x.Id == greeting.Id);
            if (existingGreeting == null)
                throw new Exception("Not found!");

            existingGreeting.Message = greeting.Message;
            existingGreeting.To = greeting.To;
            existingGreeting.From = greeting.From;

            await _greetingDbContext.SaveChangesAsync();
        }
    }
}
