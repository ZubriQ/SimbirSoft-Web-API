using System.Diagnostics.CodeAnalysis;

namespace Olymp_Project.Services
{
    public class ServiceResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }

        [MaybeNull]
        public T? Result { get; set; } = default(T?);

        public ServiceResponse(HttpStatusCode statusCode, T result)
        {
            StatusCode = statusCode;
            Result = result;
        }

        public ServiceResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }
}
