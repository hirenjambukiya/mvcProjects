namespace ELMS.Web.Helpers
{
    public static class SessionManager
    {
        public static void Set<T>(HttpContext httpContext, string key, T value)
        {
            if (value == null)
            {
                httpContext.Session.Remove(key);
                return;
            }

            var json = System.Text.Json.JsonSerializer.Serialize(value);
            httpContext.Session.SetString(key, json);
        }

        public static T? Get<T>(HttpContext httpContext, string key)
        {
            var json = httpContext.Session.GetString(key);
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
        public static bool Exists(HttpContext httpContext, string key)
        {
            return httpContext.Session.GetString(key) != null;
        }

        public static void Remove(HttpContext httpContext, string key)
        {
            httpContext.Session.Remove(key);
        }
        public static void Clear(HttpContext httpContext)
        {
            httpContext.Session.Clear();
        }
    }
}
