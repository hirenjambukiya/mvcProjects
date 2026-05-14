using System.Collections.Generic;

namespace ApiTester.Application.DTOs
{
    public class ApiResponseDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public long ResponseTime { get; set; }
        public string ResponseBody { get; set; }
        public string ErrorMessage { get; set; }
        public List<KeyValuePair<string, string>> ResponseHeaders { get; set; } = new List<KeyValuePair<string, string>>();
    }
}
