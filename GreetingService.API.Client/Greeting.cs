using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.API.Client
{
    public class Greeting
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        
    }

}
