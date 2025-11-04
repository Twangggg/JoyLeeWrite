using JoyLeeWrite.Data;
using JoyLeeWrite.Models;
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
                    .FirstOrDefault(u => u.Username == username && u.PasswordHash == password);
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
                        //PasswordHash = HashPassword(password),
                        PasswordHash = password,
                        CreatedAt = DateTime.UtcNow
                    };

                    dbContext.Users.Add(newUser);
                    dbContext.SaveChanges();
                    return newUser.UserId;
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return -2; // Lỗi DB
            }
        }

        //private string HashPassword(string password)
        //{
        //    using (var sha256 = SHA256.Create())
        //    {
        //        var bytes = Encoding.UTF8.GetBytes(password);
        //        var hash = sha256.ComputeHash(bytes);
        //        return Convert.ToBase64String(hash);
        //    }
        //}

    }
}
