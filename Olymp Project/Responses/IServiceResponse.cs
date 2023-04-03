namespace Olymp_Project.Responses
{
    public interface IServiceResponse<T>
    {
        HttpStatusCode StatusCode { get; set; }
        T? Data { get; set; }
        string? ErrorMessage { get; set; }
    }
}
