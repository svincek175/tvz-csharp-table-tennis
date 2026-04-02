using TableTennisTracker.Domain.Enums;

namespace TableTennisTracker.Domain.Models;

public class Match
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public int RoundNumber { get; set; }
    public int TableNumber { get; set; }
    public DateTime ScheduledStartUtc { get; set; }
    public DateTime? ActualStartUtc { get; set; }
    public DateTime? CompletedUtc { get; set; }
    public MatchStatus Status { get; set; }
    public int BestOfSets { get; set; }
    public Guid? WinnerPlayerId { get; set; }

    public Tournament? Tournament { get; set; }
    public Player? WinnerPlayer { get; set; }

    public ICollection<MatchParticipant> Participants { get; set; } = new List<MatchParticipant>();
    public ICollection<MatchSetResult> SetResults { get; set; } = new List<MatchSetResult>();
}