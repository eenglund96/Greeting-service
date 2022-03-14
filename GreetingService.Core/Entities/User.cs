using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core.Entities
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                EmailValidator.IsValid(value);
         
                _email = value;
            }
        }

        public string Password { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; }
        public UserApprovalStatus ApprovalStatus { get; set; }
        public string? ApprovalStatusNote { get; set; }
        public string ApprovalCode { get; set; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)).Replace("/", "").Replace("?", "");
        public DateTime ApprovalExpiry { get; set; } = DateTime.Now.AddDays(1);


        public enum UserApprovalStatus
        {
            Approved = 0,
            Rejected = 1,
            Pending = 2,
        }
    }
}
