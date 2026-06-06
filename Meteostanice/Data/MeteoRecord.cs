namespace Meteostanice.Data;

public class MeteoRecord
{
    public int Id { get; set; }
    public DateTime FetchedAt { get; set; }
    public bool IsOnline { get; set; }
    public string? JsonData { get; set; }
}