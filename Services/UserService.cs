using JoyLeeWrite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.Services
{
    internal class UserService
    {
        private JoyLeeWriteDbContext CreateContext()
        {
            return new JoyLeeWriteDbContext();
        }

        public int ValidateUserCredentials(string username, string password)
        {
            using (var dbContext = CreateContext())
            {
                var user = dbContext.Users
                    .FirstOrDefault(u => u.Username == username && u.PasswordHash == password);
                return user != null ? user.UserId : -1;
            }
        }

        public int CreateUser(string username, string password)
        {
            using (var dbContext = CreateContext())
            {
                var existingUser = dbContext.Users
                    .FirstOrDefault(u => u.Username == username);
                if (existingUser != null)
                {
                    return -1; 
                }
                var newUser = new Models.User
                {
                    Username = username,
                    PasswordHash = password
                };
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
                return newUser.UserId;
            }
        }
    }
}
