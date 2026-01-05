namespace ex_APIarchitecture.Common.constants
{
    public class cs_Response
    {
        public const string SUCCESS = "Success";
        public const string ERROR = "Internal Server Error";
        public const string NOT_FOUND = "Not Found";
        public const string UNAUTHORIZED = "Unauthorized";
        public const string FORBIDDEN = "Forbidden";
        public const string BAD_REQUEST = "Bad Request";

        public static int GetStatusCode(string responseMessage)
        {
            return responseMessage switch
            {
                SUCCESS => 200,
                ERROR => 500,
                NOT_FOUND => 404,
                UNAUTHORIZED => 401,
                FORBIDDEN => 403,
                BAD_REQUEST => 400,
                _ => 500,
            };
        }

        public const string CT_APPJSON = "application/json";
    }
}
