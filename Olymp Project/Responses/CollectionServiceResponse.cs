namespace Olymp_Project.Responses
{
    public class CollectionServiceResponse<T> : IServiceResponse<ICollection<T>>
    {
        public HttpStatusCode StatusCode { get; set; }
        public ICollection<T>? Data { get; set; } = new List<T>();
        public string? ErrorMessage { get; set; }

        public CollectionServiceResponse(HttpStatusCode statusCode, ICollection<T> data)
        {
            StatusCode = statusCode;
            Data = data;
        }

        public CollectionServiceResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Sets StatusCode to InternalServerError.
        /// </summary>
        public CollectionServiceResponse()
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }
    }
}
