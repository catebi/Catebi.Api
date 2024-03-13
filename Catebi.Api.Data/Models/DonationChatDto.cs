namespace Catebi.Api.Data.Models;

public class DonationChatDto(int id, string url, bool isConnected)
{
    public int Id { get; set; } = id;
    public string Url { get; set; } = url;
    public bool IsConnected { get; set; } = isConnected;
}