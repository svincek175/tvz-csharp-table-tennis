namespace TableTennisTracker.Domain.Models;

public class Venue
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public int NumberOfTables { get; set; }
    public int Capacity { get; set; }
    public string TimeZoneId { get; set; } = string.Empty;

    public ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}