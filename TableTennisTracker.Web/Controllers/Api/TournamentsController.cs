using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TableTennisTracker.Web.Infrastructure;
using TableTennisTracker.Web.ViewModels.Api;
using TableTennisTracker.Domain.Models;

namespace TableTennisTracker.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class TournamentsController : ControllerBase
{
    private readonly TableTennisTrackerDbContext _db;

    public TournamentsController(TableTennisTrackerDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<TournamentDto>>> Get([FromQuery] string? query)
    {
        var q = _db.Tournaments.Include(t => t.Venue).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var lower = query.Trim().ToLower();
            q = q.Where(t => t.Name.ToLower().Contains(lower) || t.SeasonLabel.ToLower().Contains(lower));
        }

        var list = await q.Select(t => new TournamentDto(t.Id, t.Name, t.SeasonLabel, t.StartUtc, t.EndUtc, t.MaxPlayers, t.BestOfSets, t.OrganizerName, t.IsRankingEvent, t.VenueId, t.Venue == null ? null : new TournamentVenueBriefDto(t.Venue.Id, t.Venue.Name, t.Venue.City))).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<TournamentDto>> GetOne(Guid id)
    {
        var t = await _db.Tournaments.Include(x => x.Venue).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (t == null) return NotFound();
        var dto = new TournamentDto(t.Id, t.Name, t.SeasonLabel, t.StartUtc, t.EndUtc, t.MaxPlayers, t.BestOfSets, t.OrganizerName, t.IsRankingEvent, t.VenueId, t.Venue == null ? null : new TournamentVenueBriefDto(t.Venue.Id, t.Venue.Name, t.Venue.City));
        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TournamentDto>> Post([FromBody] TournamentCreateDto dto)
    {
        var entity = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            SeasonLabel = dto.SeasonLabel,
            StartUtc = DateTime.SpecifyKind(dto.StartUtc, DateTimeKind.Utc),
            EndUtc = DateTime.SpecifyKind(dto.EndUtc, DateTimeKind.Utc),
            MaxPlayers = dto.MaxPlayers,
            BestOfSets = dto.BestOfSets,
            OrganizerName = dto.OrganizerName,
            IsRankingEvent = dto.IsRankingEvent,
            VenueId = dto.VenueId
        };
        _db.Tournaments.Add(entity);
        await _db.SaveChangesAsync();
        var result = new TournamentDto(entity.Id, entity.Name, entity.SeasonLabel, entity.StartUtc, entity.EndUtc, entity.MaxPlayers, entity.BestOfSets, entity.OrganizerName, entity.IsRankingEvent, entity.VenueId, null);
        return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(Guid id, [FromBody] TournamentUpdateDto dto)
    {
        var entity = await _db.Tournaments.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();
        entity.Name = dto.Name;
        entity.SeasonLabel = dto.SeasonLabel;
        entity.StartUtc = DateTime.SpecifyKind(dto.StartUtc, DateTimeKind.Utc);
        entity.EndUtc = DateTime.SpecifyKind(dto.EndUtc, DateTimeKind.Utc);
        entity.MaxPlayers = dto.MaxPlayers;
        entity.BestOfSets = dto.BestOfSets;
        entity.OrganizerName = dto.OrganizerName;
        entity.IsRankingEvent = dto.IsRankingEvent;
        entity.VenueId = dto.VenueId;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _db.Tournaments.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();
        _db.Tournaments.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
