using System;

namespace ApiTester.Domain.Entities
{
    public class ApiRequestHistory
    {
        public int Id { get; set; }
        public string ApiUrl { get; set; }
        public string HttpMethod { get; set; }
        public string RequestHeaders { get; set; }
        public string RequestJson { get; set; }
        public string ResponseJson { get; set; }
        public int StatusCode { get; set; }
        public long ResponseTime { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
