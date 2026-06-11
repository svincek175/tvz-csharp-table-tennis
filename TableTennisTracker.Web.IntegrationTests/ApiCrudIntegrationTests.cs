using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using TableTennisTracker.Domain.Enums;
using TableTennisTracker.Web.IntegrationTests.Infrastructure;
using TableTennisTracker.Web.ViewModels.Api;

namespace TableTennisTracker.Web.IntegrationTests;

[Collection("Api integration tests")]
public sealed class ApiCrudIntegrationTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;
    private readonly HttpClient _client;

    public ApiCrudIntegrationTests(ApiTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task PlayersCrudFlow_Works()
    {
        var create = new PlayerCreateDto(
            $"First-{Guid.NewGuid():N}",
            $"Last-{Guid.NewGuid():N}",
            new DateOnly(1991, 2, 3),
            "HR",
            1234,
            true);

        var created = await PostAsync<PlayerCreateDto, PlayerDto>("/api/Players", create, HttpStatusCode.Created);
        var listed = await GetAsync<List<PlayerDto>>("/api/Players");
        Assert.Contains(listed!, item => item.Id == created.Id);

        var fetched = await GetAsync<PlayerDto>($"/api/Players/{created.Id}");
        Assert.Equal(created.Id, fetched!.Id);

        var update = new PlayerUpdateDto(
            $"Updated-{Guid.NewGuid():N}",
            $"Updated-{Guid.NewGuid():N}",
            new DateOnly(1992, 4, 5),
            "SI",
            987,
            false);

        await PutAsync($"/api/Players/{created.Id}", update, HttpStatusCode.NoContent);

        var updated = await GetAsync<PlayerDto>($"/api/Players/{created.Id}");
        Assert.Equal(update.FirstName, updated!.FirstName);
        Assert.Equal(update.LastName, updated.LastName);
        Assert.Equal(update.CountryCode, updated.CountryCode);
        Assert.Equal(update.CurrentRankingPoints, updated.CurrentRankingPoints);
        Assert.Equal(update.IsActive, updated.IsActive);

        await DeleteAsync($"/api/Players/{created.Id}", HttpStatusCode.NoContent);
        await AssertMissingResourceAsync($"/api/Players/{created.Id}", update);
    }

    [Fact]
    public async Task VenuesCrudFlow_Works()
    {
        var create = new VenueCreateDto(
            $"Venue-{Guid.NewGuid():N}",
            "Zagreb",
            "HR",
            "Main street 1",
            12,
            160,
            "Europe/Zagreb");

        var created = await PostAsync<VenueCreateDto, VenueDto>("/api/Venues", create, HttpStatusCode.Created);
        var listed = await GetAsync<List<VenueDto>>("/api/Venues");
        Assert.Contains(listed!, item => item.Id == created.Id);

        var fetched = await GetAsync<VenueDto>($"/api/Venues/{created.Id}");
        Assert.Equal(created.Id, fetched!.Id);

        var update = new VenueUpdateDto(
            $"Venue-Updated-{Guid.NewGuid():N}",
            "Split",
            "HR",
            "Updated street 2",
            14,
            180,
            "Europe/Zagreb");

        await PutAsync($"/api/Venues/{created.Id}", update, HttpStatusCode.NoContent);

        var updated = await GetAsync<VenueDto>($"/api/Venues/{created.Id}");
        Assert.Equal(update.Name, updated!.Name);
        Assert.Equal(update.City, updated.City);
        Assert.Equal(update.AddressLine, updated.AddressLine);
        Assert.Equal(update.NumberOfTables, updated.NumberOfTables);
        Assert.Equal(update.Capacity, updated.Capacity);
        Assert.Equal(update.TimeZoneId, updated.TimeZoneId);

        await DeleteAsync($"/api/Venues/{created.Id}", HttpStatusCode.NoContent);
        await AssertMissingResourceAsync($"/api/Venues/{created.Id}", update);
    }

    [Fact]
    public async Task TournamentsCrudFlow_Works()
    {
        var venueId = await CreateVenueAsync();
        var create = new TournamentCreateDto(
            $"Tournament-{Guid.NewGuid():N}",
            "2026 Spring",
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow.AddDays(8),
            32,
            5,
            "Integration Tester",
            true,
            venueId);

        var created = await PostAsync<TournamentCreateDto, TournamentDto>("/api/Tournaments", create, HttpStatusCode.Created);
        var listed = await GetAsync<List<TournamentDto>>("/api/Tournaments");
        Assert.Contains(listed!, item => item.Id == created.Id);

        var fetched = await GetAsync<TournamentDto>($"/api/Tournaments/{created.Id}");
        Assert.Equal(created.Id, fetched!.Id);
        Assert.NotNull(fetched.Venue);
        Assert.Equal(venueId, fetched.VenueId);

        var update = new TournamentUpdateDto(
            $"Tournament-Updated-{Guid.NewGuid():N}",
            "2026 Autumn",
            DateTime.UtcNow.AddDays(9),
            DateTime.UtcNow.AddDays(10),
            24,
            7,
            "Integration Tester 2",
            false,
            venueId);

        await PutAsync($"/api/Tournaments/{created.Id}", update, HttpStatusCode.NoContent);

        var updated = await GetAsync<TournamentDto>($"/api/Tournaments/{created.Id}");
        Assert.Equal(update.Name, updated!.Name);
        Assert.Equal(update.SeasonLabel, updated.SeasonLabel);
        Assert.Equal(update.MaxPlayers, updated.MaxPlayers);
        Assert.Equal(update.BestOfSets, updated.BestOfSets);
        Assert.Equal(update.IsRankingEvent, updated.IsRankingEvent);

        await DeleteAsync($"/api/Tournaments/{created.Id}", HttpStatusCode.NoContent);
        await AssertMissingResourceAsync($"/api/Tournaments/{created.Id}", update);
    }

    [Fact]
    public async Task RegistrationsCrudFlow_Works()
    {
        var playerId = await CreatePlayerAsync();
        var tournamentVenueId = await CreateVenueAsync();
        var tournamentId = await CreateTournamentAsync(tournamentVenueId);

        var create = new RegistrationCreateDto(
            playerId,
            tournamentId,
            DateTime.UtcNow,
            4,
            false);

        var created = await PostAsync<RegistrationCreateDto, RegistrationDto>("/api/Registrations", create, HttpStatusCode.Created);
        var listed = await GetAsync<List<RegistrationDto>>("/api/Registrations");
        Assert.Contains(listed!, item => item.Id == created.Id);

        var fetched = await GetAsync<RegistrationDto>($"/api/Registrations/{created.Id}");
        Assert.Equal(created.Id, fetched!.Id);
        Assert.NotNull(fetched.Player);
        Assert.Equal(playerId, fetched.PlayerId);

        var update = new RegistrationUpdateDto(
            playerId,
            tournamentId,
            DateTime.UtcNow.AddHours(1),
            7,
            true);

        await PutAsync($"/api/Registrations/{created.Id}", update, HttpStatusCode.NoContent);

        var updated = await GetAsync<RegistrationDto>($"/api/Registrations/{created.Id}");
        Assert.Equal(update.SeedNumber, updated!.SeedNumber);
        Assert.Equal(update.IsCheckedIn, updated.IsCheckedIn);

        await DeleteAsync($"/api/Registrations/{created.Id}", HttpStatusCode.NoContent);
        await AssertMissingResourceAsync($"/api/Registrations/{created.Id}", update);
    }

    [Fact]
    public async Task MatchesCrudFlow_Works()
    {
        var venueId = await CreateVenueAsync();
        var tournamentId = await CreateTournamentAsync(venueId);

        var create = new MatchCreateDto(
            tournamentId,
            1,
            5,
            DateTime.UtcNow.AddDays(1),
            null,
            null,
            5,
            null,
            MatchStatus.Scheduled.ToString());

        var created = await PostAsync<MatchCreateDto, MatchDto>("/api/Matches", create, HttpStatusCode.Created);
        var listed = await GetAsync<List<MatchDto>>("/api/Matches");
        Assert.Contains(listed!, item => item.Id == created.Id);

        var fetched = await GetAsync<MatchDto>($"/api/Matches/{created.Id}");
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal(MatchStatus.Scheduled.ToString(), fetched.Status);

        var update = new MatchUpdateDto(
            tournamentId,
            2,
            8,
            DateTime.UtcNow.AddDays(2),
            DateTime.UtcNow.AddDays(2).AddMinutes(5),
            DateTime.UtcNow.AddDays(2).AddHours(1),
            7,
            null,
            MatchStatus.Completed.ToString());

        await PutAsync($"/api/Matches/{created.Id}", update, HttpStatusCode.NoContent);

        var updated = await GetAsync<MatchDto>($"/api/Matches/{created.Id}");
        Assert.Equal(update.RoundNumber, updated!.RoundNumber);
        Assert.Equal(update.TableNumber, updated.TableNumber);
        Assert.Equal(MatchStatus.Completed.ToString(), updated.Status);

        await DeleteAsync($"/api/Matches/{created.Id}", HttpStatusCode.NoContent);
        await AssertMissingResourceAsync($"/api/Matches/{created.Id}", update);
    }

    [Fact]
    public async Task MatchParticipantsCrudFlow_Works()
    {
        var venueId = await CreateVenueAsync();
        var tournamentId = await CreateTournamentAsync(venueId);
        var matchId = await CreateMatchAsync(tournamentId);
        var playerId = await CreatePlayerAsync();

        var create = new MatchParticipantCreateDto(matchId, playerId, 1, 3);
        var created = await PostAsync<MatchParticipantCreateDto, MatchParticipantDto>("/api/MatchParticipants", create, HttpStatusCode.Created);
        var listed = await GetAsync<List<MatchParticipantDto>>("/api/MatchParticipants?matchId=" + matchId);
        Assert.Contains(listed!, item => item.Id == created.Id);

        var fetched = await GetAsync<MatchParticipantDto>($"/api/MatchParticipants/{created.Id}");
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal(playerId, fetched.PlayerId);

        var update = new MatchParticipantUpdateDto(matchId, playerId, 2, 4);
        await PutAsync($"/api/MatchParticipants/{created.Id}", update, HttpStatusCode.NoContent);

        var updated = await GetAsync<MatchParticipantDto>($"/api/MatchParticipants/{created.Id}");
        Assert.Equal(update.Slot, updated!.Slot);
        Assert.Equal(update.ScoreSetsWon, updated.ScoreSetsWon);

        await DeleteAsync($"/api/MatchParticipants/{created.Id}", HttpStatusCode.NoContent);
        await AssertMissingResourceAsync($"/api/MatchParticipants/{created.Id}", update);
    }

    [Fact]
    public async Task MatchSetResultsCrudFlow_Works()
    {
        var venueId = await CreateVenueAsync();
        var tournamentId = await CreateTournamentAsync(venueId);
        var matchId = await CreateMatchAsync(tournamentId);

        var create = new MatchSetResultCreateDto(matchId, 1, 11, 8);
        var created = await PostAsync<MatchSetResultCreateDto, MatchSetResultDto>("/api/MatchSetResults", create, HttpStatusCode.Created);
        var listed = await GetAsync<List<MatchSetResultDto>>("/api/MatchSetResults?matchId=" + matchId);
        Assert.Contains(listed!, item => item.Id == created.Id);

        var fetched = await GetAsync<MatchSetResultDto>($"/api/MatchSetResults/{created.Id}");
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal(1, fetched.SetNumber);

        var update = new MatchSetResultUpdateDto(matchId, 2, 9, 11);
        await PutAsync($"/api/MatchSetResults/{created.Id}", update, HttpStatusCode.NoContent);

        var updated = await GetAsync<MatchSetResultDto>($"/api/MatchSetResults/{created.Id}");
        Assert.Equal(update.SetNumber, updated!.SetNumber);
        Assert.Equal(update.PlayerOnePoints, updated.PlayerOnePoints);
        Assert.Equal(update.PlayerTwoPoints, updated.PlayerTwoPoints);

        await DeleteAsync($"/api/MatchSetResults/{created.Id}", HttpStatusCode.NoContent);
        await AssertMissingResourceAsync($"/api/MatchSetResults/{created.Id}", update);
    }

    [Fact]
    public async Task QuizFilesCrudFlow_Works()
    {
        var fileBytes = Encoding.UTF8.GetBytes("integration-test-file");
        using var uploadContent = new MultipartFormDataContent();
        uploadContent.Add(new ByteArrayContent(fileBytes), "file", "quiz-upload.txt");

        var uploadResponse = await _client.PostAsync("/api/QuizFiles", uploadContent);
        Assert.Equal(HttpStatusCode.Created, uploadResponse.StatusCode);

        var created = await uploadResponse.Content.ReadFromJsonAsync<QuizFileDto>();
        Assert.NotNull(created);
        Assert.True(File.Exists(Path.Combine(_factory.WebRootPath, created!.RelativePath.Replace('/', Path.DirectorySeparatorChar))));

        var listed = await GetAsync<List<QuizFileDto>>("/api/QuizFiles");
        Assert.Contains(listed!, item => item.Id == created.Id);

        var fetched = await GetAsync<QuizFileDto>($"/api/QuizFiles/{created.Id}");
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal("quiz-upload.txt", fetched.OriginalFileName);

        await DeleteAsync($"/api/QuizFiles/{created.Id}", HttpStatusCode.NoContent);
        Assert.False(File.Exists(Path.Combine(_factory.WebRootPath, created.RelativePath.Replace('/', Path.DirectorySeparatorChar))));

        var missing = await _client.GetAsync($"/api/QuizFiles/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task QuizFilesUploadWithoutFile_ReturnsBadRequest()
    {
        using var content = new MultipartFormDataContent();
        var response = await _client.PostAsync("/api/QuizFiles", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PlayersSearchSupportsFieldFilter()
    {
        var matchingPlayer = await PostAsync<PlayerCreateDto, PlayerDto>("/api/Players", new PlayerCreateDto(
            $"Filter-{Guid.NewGuid():N}",
            "Match",
            new DateOnly(1991, 1, 1),
            "HR",
            100,
            true), HttpStatusCode.Created);

        var otherPlayer = await PostAsync<PlayerCreateDto, PlayerDto>("/api/Players", new PlayerCreateDto(
            $"Other-{Guid.NewGuid():N}",
            "Match",
            new DateOnly(1992, 2, 2),
            "US",
            200,
            true), HttpStatusCode.Created);

        var html = await GetHtmlAsync($"/players/search?query={Uri.EscapeDataString(matchingPlayer.FirstName)}&filter=name");

        Assert.Contains(matchingPlayer.FirstName, html);
        Assert.DoesNotContain(otherPlayer.FirstName, html);
    }

    [Fact]
    public async Task VenuesSearchSupportsFieldFilter()
    {
        var matchingVenue = await PostAsync<VenueCreateDto, VenueDto>("/api/Venues", new VenueCreateDto(
            $"Venue-{Guid.NewGuid():N}",
            $"City-{Guid.NewGuid():N}",
            "HR",
            "Filter street",
            4,
            40,
            "Europe/Zagreb"), HttpStatusCode.Created);

        var otherVenue = await PostAsync<VenueCreateDto, VenueDto>("/api/Venues", new VenueCreateDto(
            $"Venue-{Guid.NewGuid():N}",
            $"OtherCity-{Guid.NewGuid():N}",
            "SI",
            "Other street",
            6,
            60,
            "Europe/Ljubljana"), HttpStatusCode.Created);

        var html = await GetHtmlAsync($"/venues/search?query={Uri.EscapeDataString(matchingVenue.City)}&filter=city");

        Assert.Contains(matchingVenue.City, html);
        Assert.DoesNotContain(otherVenue.City, html);
    }

    private async Task<Guid> CreateVenueAsync()
    {
        var create = new VenueCreateDto(
            $"Venue-{Guid.NewGuid():N}",
            "Zagreb",
            "HR",
            "Integration street",
            8,
            80,
            "Europe/Zagreb");

        var venue = await PostAsync<VenueCreateDto, VenueDto>("/api/Venues", create, HttpStatusCode.Created);
        return venue.Id;
    }

    private async Task<Guid> CreateTournamentAsync(Guid venueId)
    {
        var create = new TournamentCreateDto(
            $"Tournament-{Guid.NewGuid():N}",
            "2026 Integration",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(2),
            16,
            5,
            "Integration Suite",
            true,
            venueId);

        var tournament = await PostAsync<TournamentCreateDto, TournamentDto>("/api/Tournaments", create, HttpStatusCode.Created);
        return tournament.Id;
    }

    private async Task<Guid> CreatePlayerAsync()
    {
        var create = new PlayerCreateDto(
            $"Player-{Guid.NewGuid():N}",
            $"User-{Guid.NewGuid():N}",
            new DateOnly(1990, 1, 1),
            "HR",
            1000,
            true);

        var player = await PostAsync<PlayerCreateDto, PlayerDto>("/api/Players", create, HttpStatusCode.Created);
        return player.Id;
    }

    private async Task<Guid> CreateMatchAsync(Guid tournamentId)
    {
        var create = new MatchCreateDto(
            tournamentId,
            1,
            1,
            DateTime.UtcNow.AddHours(1),
            null,
            null,
            5,
            null,
            MatchStatus.Scheduled.ToString());

        var match = await PostAsync<MatchCreateDto, MatchDto>("/api/Matches", create, HttpStatusCode.Created);
        return match.Id;
    }

    private async Task<TResponse?> GetAsync<TResponse>(string path)
    {
        var response = await _client.GetAsync(path);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest payload, HttpStatusCode expectedStatus)
    {
        using var content = JsonContent.Create(payload);
        var response = await _client.PostAsync(path, content);
        Assert.Equal(expectedStatus, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<TResponse>();
        Assert.NotNull(result);
        return result!;
    }

    private async Task<string> GetHtmlAsync(string path)
    {
        var response = await _client.GetAsync(path);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadAsStringAsync();
    }

    private async Task PutAsync<TRequest>(string path, TRequest payload, HttpStatusCode expectedStatus)
    {
        using var content = JsonContent.Create(payload);
        var response = await _client.PutAsync(path, content);
        Assert.Equal(expectedStatus, response.StatusCode);
    }

    private async Task DeleteAsync(string path, HttpStatusCode expectedStatus)
    {
        var response = await _client.DeleteAsync(path);
        Assert.Equal(expectedStatus, response.StatusCode);
    }

    private async Task AssertMissingResourceAsync<TRequest>(string path, TRequest updatePayload)
    {
        var getResponse = await _client.GetAsync(path);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        using var putContent = JsonContent.Create(updatePayload);
        var putResponse = await _client.PutAsync(path, putContent);
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);

        var deleteResponse = await _client.DeleteAsync(path);
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }
}
