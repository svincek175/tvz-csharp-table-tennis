namespace TableTennisTracker.Domain.Models;

public class MatchSetResult
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public int SetNumber { get; set; }
    public int PlayerOnePoints { get; set; }
    public int PlayerTwoPoints { get; set; }

    public Match? Match { get; set; }
}