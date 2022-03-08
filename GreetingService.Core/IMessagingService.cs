using System;
using GreetingService.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreetingService.Core.Enum;

namespace GreetingService.Core
{
    public interface IMessagingService
    {
       public Task SendAsync<T>(T message, MessagingServiceSubject subject);
    }
}
