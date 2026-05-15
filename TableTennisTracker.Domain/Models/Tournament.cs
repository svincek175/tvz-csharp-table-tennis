using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TableTennisTracker.Domain.Models;

public class Tournament
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string SeasonLabel { get; set; } = string.Empty;

    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public int MaxPlayers { get; set; }
    public int BestOfSets { get; set; }

    [MaxLength(200)]
    public string OrganizerName { get; set; } = string.Empty;

    public bool IsRankingEvent { get; set; }

    public Guid VenueId { get; set; }

    [ForeignKey(nameof(VenueId))]
    public virtual Venue? Venue { get; set; }

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
}