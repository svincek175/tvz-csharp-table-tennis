using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TableTennisTracker.Domain.Models;

public class Registration
{
    [Key]
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }
    public Guid TournamentId { get; set; }
    public DateTime RegisteredUtc { get; set; }
    public int SeedNumber { get; set; }
    public bool IsCheckedIn { get; set; }

    [ForeignKey(nameof(PlayerId))]
    public virtual Player? Player { get; set; }

    [ForeignKey(nameof(TournamentId))]
    public virtual Tournament? Tournament { get; set; }
}