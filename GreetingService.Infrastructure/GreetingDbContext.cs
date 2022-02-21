using GreetingService.API.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure
{
    public class GreetingDbContext : DbContext
    {
        public GreetingDbContext(DbContextOptions options) : base(options)
        {
        }

        public GreetingDbContext()
        {
        }

        public DbSet<Greeting> Greetings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("GreetingDbConnectionString"));
        }
    }
}
