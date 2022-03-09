using GreetingService.API.Core;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core.Entities
{
    public class Invoice
    {
        public int? Id { get; set; }
        public User Sender { get; set; }
        public IEnumerable<Greeting> Greetings { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal CostPerGreeting { get; set; } = 25;

        private decimal _totalCost;
        public decimal TotalCost
        {
            get
            {
                if (Greetings == null)
                    return 0;

                return Greetings.Count() * CostPerGreeting;
            }
            set
            {
                _totalCost = value;
            }
        }
        public string Currency { get; set; } = "SEK";
    }
}
