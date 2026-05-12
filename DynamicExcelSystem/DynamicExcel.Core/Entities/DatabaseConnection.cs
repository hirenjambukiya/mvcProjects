using System;

namespace DynamicExcel.Core.Entities
{
    public class DatabaseConnection
    {
        public int Id { get; set; }
        public string ConnectionName { get; set; }
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string AuthenticationType { get; set; } // "Windows" or "SQL"
        public string Username { get; set; }
        public string Password { get; set; } // Encrypted in real world
        public bool IsDefault { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public string GetConnectionString()
        {
            if (AuthenticationType == "Windows")
            {
                return $"Server={ServerName};Database={DatabaseName};Trusted_Connection=True;TrustServerCertificate=True;";
            }
            else
            {
                return $"Server={ServerName};Database={DatabaseName};User Id={Username};Password={Password};TrustServerCertificate=True;";
            }
        }
    }
}
