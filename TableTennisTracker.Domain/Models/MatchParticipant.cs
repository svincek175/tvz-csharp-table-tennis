using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TableTennisTracker.Domain.Models;

public class MatchParticipant
{
    [Key]
    public Guid Id { get; set; }

    public Guid MatchId { get; set; }
    public Guid PlayerId { get; set; }
    public int Slot { get; set; }
    public int ScoreSetsWon { get; set; }

    [ForeignKey(nameof(MatchId))]
    public virtual Match? Match { get; set; }

    [ForeignKey(nameof(PlayerId))]
    public virtual Player? Player { get; set; }
}