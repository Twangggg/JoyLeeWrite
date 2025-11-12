using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace JoyLeeWrite.Utils
{
   

    public static class SecurityHelper
    {
        private static readonly PasswordHasher<object> hasher = new PasswordHasher<object>();
        public static string HashPassword(string password)
        {
            return hasher.HashPassword(null, password);
        }

        public static bool VerifyPassword(string password, string hashedPasswordFromDb)
        {
            var result = hasher.VerifyHashedPassword(null, hashedPasswordFromDb, password);

            // Trả về true nếu password khớp
            return result == PasswordVerificationResult.Success;
        }
    }
}
