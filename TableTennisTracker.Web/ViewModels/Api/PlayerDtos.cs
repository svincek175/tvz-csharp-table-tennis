using System;

namespace TableTennisTracker.Web.ViewModels.Api
{
    public record PlayerDto(Guid Id, string FirstName, string LastName, DateOnly DateOfBirth, string CountryCode, int CurrentRankingPoints, bool IsActive);

    public record PlayerCreateDto(string FirstName, string LastName, DateOnly DateOfBirth, string CountryCode, int CurrentRankingPoints, bool IsActive);

    public record PlayerUpdateDto(string FirstName, string LastName, DateOnly DateOfBirth, string CountryCode, int CurrentRankingPoints, bool IsActive);
}
