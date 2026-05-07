using System.ComponentModel.DataAnnotations;

namespace TableTennisTracker.Domain.Models;

public class Player
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    [Required]
    [MaxLength(2)]
    public string CountryCode { get; set; } = string.Empty;

    public int CurrentRankingPoints { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedUtc { get; set; }

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public virtual ICollection<MatchParticipant> MatchParticipations { get; set; } = new List<MatchParticipant>();
}