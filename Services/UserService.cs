using JoyLeeWrite.Data;
using JoyLeeWrite.Models;
using JoyLeeWrite.Utils;
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

        public User? ValidateUserCredentials(string username, string password)
        {
            using (var dbContext = CreateContext())
            {
                var user = dbContext.Users
                    .FirstOrDefault(u => u.Username == username);
                return user == null ? null : user;
            }
        }

        public int CreateUser(string username, string password)
        {
            try
            {
                using (var dbContext = CreateContext())
                {
                    if (dbContext.Users.Any(u => u.Username.ToLower() == username.ToLower()))
                        return -1; // User đã tồn tại

                    var newUser = new Models.User
                    {
                        Username = username,
                        PasswordHash = SecurityHelper.HashPassword(password),
                        CreatedAt = DateTime.UtcNow
                    };

                    dbContext.Users.Add(newUser);
                    dbContext.SaveChanges();
                    return newUser.UserId;
                }
            }
            catch (Exception ex)
            {
                return -2;
            }
        }

        public bool UpdateUserProfile(int userId, string newUsername, string newEmail)
        {
            using (var dbContext = CreateContext())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                    return false;
                user.Username = newUsername;
                user.Email = newEmail;
                dbContext.SaveChanges();
                return true;
            }
        }

        public bool UpdatePasswordByEmail(string email, string password)
        {
            using (var dbContext = CreateContext())
            {
                var user = dbContext.Users.FirstOrDefault(
                    u => u.Email == email);
                if (user == null) return false;
                user.PasswordHash = SecurityHelper.HashPassword(password);
                dbContext.SaveChanges();
                return true;
            }        
        }
        public bool CheckExistEmail(string email)
        {
            using (var dbContext = CreateContext())
            {
                return dbContext.Set<User>().Any(u => u.Email != null &&
                                              u.Email.ToLower().Equals(email.ToLower()));
            }
        }

        public bool CheckExistUsername(string username)
        {
            using (var dbContext = CreateContext())
            {
                return dbContext.Set<User>().Any(u => u.Username.ToLower().Equals(username.ToLower()));
            }
        }
    }
}
