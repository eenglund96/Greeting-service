using GreetingService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core
{
    public interface IInvoiceService
    {
        public Task<IEnumerable<Invoice>> GetInvoicesAsync(int year, int month);
        public Task<Invoice> GetInvoiceAsync(string email, int year, int month);
        public Task CreateOrUpdateInvoiceAsync(Invoice invoice);
    }
}
