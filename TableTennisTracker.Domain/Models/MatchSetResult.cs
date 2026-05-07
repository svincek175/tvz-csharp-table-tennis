using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TableTennisTracker.Domain.Models;

public class MatchSetResult
{
    [Key]
    public Guid Id { get; set; }

    public Guid MatchId { get; set; }
    public int SetNumber { get; set; }
    public int PlayerOnePoints { get; set; }
    public int PlayerTwoPoints { get; set; }

    [ForeignKey(nameof(MatchId))]
    public virtual Match? Match { get; set; }
}