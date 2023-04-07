namespace Olymp_Project.Responses
{
    public class ServiceResponse<T> : IServiceResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public T? Data { get; set; } = default;
        public string? ErrorMessage { get; set; }

        public ServiceResponse(HttpStatusCode statusCode, T data)
        {
            StatusCode = statusCode;
            Data = data;
        }

        public ServiceResponse(HttpStatusCode statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
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
