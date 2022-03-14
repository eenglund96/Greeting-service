using GreetingService.Core;
using GreetingService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GreetingService.Core.Entities.User;

namespace GreetingService.Infrastructure.UserService
{
    public class SqlUserService : IUserService
    {
        private readonly GreetingDbContext _greetingDbContext;
        private readonly ILogger<SqlUserService> _logger;
        public SqlUserService(GreetingDbContext greetingDbContext, ILogger<SqlUserService> logger)
        {
            _greetingDbContext = greetingDbContext;
            _logger = logger;
        }
        public async Task CreateUserAsync(User user)
        {
            if (await _greetingDbContext.Users.AnyAsync(x => x.Email == user.Email && x.ApprovalStatus == UserApprovalStatus.Approved))
                return;

            var existingUnapprovedUser = await _greetingDbContext.Users.FirstOrDefaultAsync(x => x.Email == user.Email && x.ApprovalStatus != UserApprovalStatus.Approved);
            if (existingUnapprovedUser != null)
                _greetingDbContext.Users.Remove(existingUnapprovedUser);

            user.Created = DateTime.Now;
            user.Updated = DateTime.Now;
            user.ApprovalStatus = UserApprovalStatus.Pending;
            user.ApprovalStatusNote = "Awaiting approval from admin!";
            await _greetingDbContext.Users.AddAsync(user);
            await _greetingDbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string email)
        {
            var user = await _greetingDbContext.Users.FirstOrDefaultAsync(x => x.Email.Equals(email));
            if (user == null)
            {
                _logger.LogWarning("Delete user failed, user with email {email} not found!", email);
                throw new KeyNotFoundException();
            }
                
            _greetingDbContext.Users.Remove(user);
            await _greetingDbContext.SaveChangesAsync();
        }

        public async Task <User> GetUserAsync(string email)
        {
            var user = await _greetingDbContext.Users.FirstOrDefaultAsync(x => x.Email.Equals(email));
            if (user == null)
            {
                _logger.LogWarning("Could not find a user with email {email}!", email);
                throw new KeyNotFoundException();
            }

            return user;
        }

        public async Task <IEnumerable<User>> GetUsersAsync()
        {
            return await _greetingDbContext.Users.ToListAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _greetingDbContext.Users.FirstOrDefaultAsync(x => x.Email.Equals(user.Email));
            if (existingUser == null)
            {
                _logger.LogWarning("Update user failed, user with email {email} could not found!", user.Email);
                throw new KeyNotFoundException("User not found!");
            }

            if (!string.IsNullOrWhiteSpace(user.Password))
                existingUser.Password = user.Password;

            if (!string.IsNullOrWhiteSpace(user.LastName))
                existingUser.LastName = user.LastName;

            if (!string.IsNullOrWhiteSpace(user.FirstName))
                existingUser.FirstName = user.FirstName;

            existingUser.Updated = DateTime.Now;
            await _greetingDbContext.SaveChangesAsync();
        }

        public bool IsValidUser(string username, string password)
        {
            var user = _greetingDbContext.Users.FirstOrDefault(x => x.Email.Equals(username));
            if (user != null && user.Password.Equals(password))
                return true;

            return false;
        }

        public async Task<bool> IsValidUserAsync(string username, string password)
        {
            var user = await _greetingDbContext.Users.FirstOrDefaultAsync(x => x.Email.Equals(username));
            if (user != null && user.Password.Equals(password))
                return true;

            return false;
        }

        public async Task ApproveUserAsync(string approvalCode)
        {
            var user = await GetUserForApprovalAsync(approvalCode);

            user.ApprovalStatus = UserApprovalStatus.Approved;
            user.ApprovalStatusNote = $"Approved by an administrator at {DateTime.Now:O}";  
            await _greetingDbContext.SaveChangesAsync();
        }

        private async Task<User> GetUserForApprovalAsync(string approvalCode)
        {
            var user = await _greetingDbContext.Users.FirstOrDefaultAsync(x => x.ApprovalStatus == UserApprovalStatus.Pending && x.ApprovalCode.Equals(approvalCode) && x.ApprovalExpiry > DateTime.Now);
            if (user == null)
            {
                _logger.LogError($"User with approval code: {approvalCode} could not be found!");
                throw new Exception($"User with approval code: {approvalCode} could not be found!");

            }

            return user;
        }

        public async Task RejectUserAsync(string approvalCode)
        {
            var user = await GetUserForApprovalAsync(approvalCode);

            user.ApprovalStatus = UserApprovalStatus.Rejected;
            user.ApprovalStatusNote = $"The user was rejected by an administrator at {DateTime.Now:O}";
            await _greetingDbContext.SaveChangesAsync();
        }
    }
}
