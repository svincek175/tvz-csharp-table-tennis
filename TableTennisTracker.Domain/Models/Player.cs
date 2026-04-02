namespace TableTennisTracker.Domain.Models;

public class Player
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public int CurrentRankingPoints { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedUtc { get; set; }

    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public ICollection<MatchParticipant> MatchParticipations { get; set; } = new List<MatchParticipant>();
}