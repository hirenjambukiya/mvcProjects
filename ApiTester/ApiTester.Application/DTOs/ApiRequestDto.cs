using System.Collections.Generic;

namespace ApiTester.Application.DTOs
{
    public class ApiRequestDto
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public List<KeyValuePair<string, string>> Headers { get; set; } = new List<KeyValuePair<string, string>>();
        public string Body { get; set; }
    }
}
