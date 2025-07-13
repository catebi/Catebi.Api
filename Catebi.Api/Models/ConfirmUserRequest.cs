namespace Catebi.Api.Models;

public class ConfirmUserRequest
{
    public string RecordId { get; set; }
    public bool IsVolunteer { get; set; }
    public string? Notes { get; set; }
}
