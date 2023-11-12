namespace Catebi.Api.Data.Models;

public class CatDtoShort(int id, string geoLocation)
{
    public int Id { get; set; } = id;
    public string GeoLocation { get; set; } = geoLocation;
}
