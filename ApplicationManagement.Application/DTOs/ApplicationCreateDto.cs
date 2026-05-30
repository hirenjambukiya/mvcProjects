using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AMS.Application.DTOs
{
    public class ApplicationCreateDto
    {
      
        public string Name { get; set; } = string.Empty;

       
        public int Age { get; set; }

        
        public string Gender { get; set; } = string.Empty;

       
        public string Country { get; set; } = string.Empty;

      
        public string State { get; set; } = string.Empty;

       
        public string District { get; set; } = string.Empty;

       
        public string Pincode { get; set; } = string.Empty;

        
        public string Address { get; set; } = string.Empty;

        public IFormFile? File { get; set; }
    }
}
