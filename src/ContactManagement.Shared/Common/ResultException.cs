using System.Net;

namespace ContactManagement.Shared.Common
{
    public class ResultException(Error error, HttpStatusCode statusCode) : Exception(error.Message)
    {
        public Error Error { get; } = error;
        public HttpStatusCode StatusCode { get; } = statusCode;
    }
}

