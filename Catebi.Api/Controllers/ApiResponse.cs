namespace Catebi.Api.Controllers;

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
} 