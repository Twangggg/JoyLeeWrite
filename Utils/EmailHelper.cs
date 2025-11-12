using JoyLeeWrite.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JoyLeeWrite.Utils
{
    public class EmailHelper
    {
        public static string CurrentOtp { get; set; }
        public static async Task SendOtpAsync(string email)
        {
            var message = new MailMessage();
            string otp = GenerateOtp();
            CurrentOtp = otp;
            message.To.Add(email);
            message.From = new MailAddress("tktoan10a1@gmail.com");
            message.Subject = "Your verification code";
            message.Body = $"Your OTP: {otp}";

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("tktoan10a1@gmail.com", "gkbd pwip lych kfvm"),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
        }
        public static string GenerateOtp()
        {
            var rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }
    }
}
