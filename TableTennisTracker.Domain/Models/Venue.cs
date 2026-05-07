using System.ComponentModel.DataAnnotations;

namespace TableTennisTracker.Domain.Models;

public class Venue
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(2)]
    public string CountryCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    public string AddressLine { get; set; } = string.Empty;

    public int NumberOfTables { get; set; }
    public int Capacity { get; set; }

    [Required]
    [MaxLength(100)]
    public string TimeZoneId { get; set; } = string.Empty;

    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}