using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GreetingService.Core
{
    public class EmailValidator
    {
        private static Regex _regex { get; set; } = new Regex(@"^[a-zA-Z0-9\._-]{2,25}.@.[a-z]{1,12}.(com|org|co\.in|net|se|uk)");

        public static bool IsValid(string email)
        {
            if (!_regex.IsMatch(email))
            {
                throw new Exception("The email address is not valid!");
            }

            return true;
        }
    }
}
