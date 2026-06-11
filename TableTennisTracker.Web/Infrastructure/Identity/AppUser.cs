using Microsoft.AspNetCore.Identity;

namespace TableTennisTracker.Web.Infrastructure.Identity;

public class AppUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePhotoPath { get; set; }
    public DateTime CreatedUtc { get; set; }
    public bool IsActive { get; set; } = true;

    public string FullName => $"{FirstName} {LastName}".Trim();
}
