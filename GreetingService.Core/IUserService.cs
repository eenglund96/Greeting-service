using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core
{
    public interface IUserService
    {
        public bool IsValidUser(string username, string password);
    }
}
