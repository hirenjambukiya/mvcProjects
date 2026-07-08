using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Application.Security
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            try
            {
                using SHA256 sha256 = SHA256.Create();

                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();

                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
