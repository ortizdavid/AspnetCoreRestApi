
using System.Net;

namespace AspNetCoreRestApi.Helpers
{
    public class JsonResponse
    {
        public static object Build(object data, HttpStatusCode statusCode)
        {
            return new { Data = data, Status = statusCode };
        }

        public static object Build(HttpStatusCode statusCode)
        {
            return new { Status = statusCode };
        }
    }
}