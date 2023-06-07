namespace SimbirSoft_Web_API.Responses
{
    public interface IResponse<T>
    {
        HttpStatusCode StatusCode { get; set; }
        T? Data { get; set; }
        string? ErrorMessage { get; set; }
    }
}
