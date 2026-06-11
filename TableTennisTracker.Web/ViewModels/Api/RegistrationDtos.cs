using System;

namespace TableTennisTracker.Web.ViewModels.Api
{
    public record RegistrationPlayerBriefDto(Guid Id, string FirstName, string LastName);

    public record RegistrationDto(Guid Id, Guid PlayerId, Guid TournamentId, DateTime RegisteredUtc, int SeedNumber, bool IsCheckedIn, RegistrationPlayerBriefDto? Player = null);

    public record RegistrationCreateDto(Guid PlayerId, Guid TournamentId, DateTime RegisteredUtc, int SeedNumber, bool IsCheckedIn);

    public record RegistrationUpdateDto(Guid PlayerId, Guid TournamentId, DateTime RegisteredUtc, int SeedNumber, bool IsCheckedIn);
}
