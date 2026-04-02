namespace TableTennisTracker.Domain.Models;

public class MatchParticipant
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public Guid PlayerId { get; set; }
    public int Slot { get; set; }
    public int ScoreSetsWon { get; set; }

    public Match? Match { get; set; }
    public Player? Player { get; set; }
}