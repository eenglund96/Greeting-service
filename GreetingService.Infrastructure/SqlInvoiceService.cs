using GreetingService.Core;
using GreetingService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure
{
    public class SqlInvoiceService : IInvoiceService
    {
        private readonly GreetingDbContext _greetingDbContext;

        public SqlInvoiceService(GreetingDbContext greetingDbContext)
        {
            _greetingDbContext = greetingDbContext;
        }

        public async Task CreateOrUpdateInvoiceAsync(Invoice invoice)
        {
            var existingInvoice = await _greetingDbContext.Invoices.FirstOrDefaultAsync(x => x.Year == invoice.Year && x.Month == invoice.Month && x.Sender.Email.Equals(invoice.Sender.Email));
            if (existingInvoice == null)
            {
                await _greetingDbContext.AddAsync(invoice);
                await _greetingDbContext.SaveChangesAsync();
            }
            else
            {
                existingInvoice.Greetings = invoice.Greetings;
                existingInvoice.TotalCost = invoice.TotalCost;
                await _greetingDbContext.SaveChangesAsync();
            }
        }

        public async Task<Invoice> GetInvoiceAsync(string email, int year, int month)
        {
            var invoice = await _greetingDbContext.Invoices.Include(x => x.Greetings)
                                                           .Include(x => x.Sender)
                                                           .FirstOrDefaultAsync(x => x.Year == year && x.Month == month && x.Sender.Email.Equals(email));
            //if (invoice == null)
            //{
            //    throw new Exception("An exception occured!");
            //}

            return invoice;
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesAsync(int year, int month)
        {
            var invoices = await _greetingDbContext.Invoices.Include(x => x.Greetings)
                                                            .Include(x => x.Sender)
                                                            .Where(x => x.Year == year && x.Month == month).ToListAsync();
            return invoices;
        }
    }
}
