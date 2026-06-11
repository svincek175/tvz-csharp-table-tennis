using System;

namespace TableTennisTracker.Web.ViewModels.Api
{
    public record VenueDto(Guid Id, string Name, string City, string CountryCode, string AddressLine, int NumberOfTables, int Capacity, string TimeZoneId);

    public record VenueCreateDto(string Name, string City, string CountryCode, string AddressLine, int NumberOfTables, int Capacity, string TimeZoneId);

    public record VenueUpdateDto(string Name, string City, string CountryCode, string AddressLine, int NumberOfTables, int Capacity, string TimeZoneId);
}
