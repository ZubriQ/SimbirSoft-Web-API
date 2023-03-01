using System.Diagnostics.CodeAnalysis;

namespace Olymp_Project.Responses
{
    public class ServiceResponse<T> : IServiceResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public T? Data { get; set; } = default;

        public ServiceResponse(HttpStatusCode statusCode, T data)
        {
            StatusCode = statusCode;
            Data = data;
        }

        public ServiceResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Sets StatusCode to InternalServerError.
        /// </summary>
        public ServiceResponse()
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }
    }
}
