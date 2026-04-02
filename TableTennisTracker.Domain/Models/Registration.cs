namespace TableTennisTracker.Domain.Models;

public class Registration
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public Guid TournamentId { get; set; }
    public DateTime RegisteredUtc { get; set; }
    public int SeedNumber { get; set; }
    public bool IsCheckedIn { get; set; }

    public Player? Player { get; set; }
    public Tournament? Tournament { get; set; }
}