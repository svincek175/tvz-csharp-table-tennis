using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TableTennisTracker.Domain.Enums;

namespace TableTennisTracker.Domain.Models;

public class Match
{
    [Key]
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

    [ForeignKey(nameof(TournamentId))]
    public virtual Tournament? Tournament { get; set; }

    [ForeignKey(nameof(WinnerPlayerId))]
    public virtual Player? WinnerPlayer { get; set; }

    public virtual ICollection<MatchParticipant> Participants { get; set; } = new List<MatchParticipant>();
    public virtual ICollection<MatchSetResult> SetResults { get; set; } = new List<MatchSetResult>();
}