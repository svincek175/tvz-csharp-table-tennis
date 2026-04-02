namespace TableTennisTracker.Domain.Models;

public class Tournament
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SeasonLabel { get; set; } = string.Empty;
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public int MaxPlayers { get; set; }
    public int BestOfSets { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public bool IsRankingEvent { get; set; }

    public Guid VenueId { get; set; }
    public Venue? Venue { get; set; }

    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}