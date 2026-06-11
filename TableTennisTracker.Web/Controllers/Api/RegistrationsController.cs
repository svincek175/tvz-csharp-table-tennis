using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TableTennisTracker.Web.Infrastructure;
using TableTennisTracker.Web.ViewModels.Api;
using TableTennisTracker.Domain.Models;

namespace TableTennisTracker.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly TableTennisTrackerDbContext _db;

    public RegistrationsController(TableTennisTrackerDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RegistrationDto>>> Get([FromQuery] string? query)
    {
        var q = _db.Registrations.Include(r => r.Player).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var lower = query.Trim().ToLower();
            q = q.Where(r => r.Player != null && (r.Player.FirstName.ToLower().Contains(lower) || r.Player.LastName.ToLower().Contains(lower)));
        }

        var list = await q.Select(r => new RegistrationDto(r.Id, r.PlayerId, r.TournamentId, r.RegisteredUtc, r.SeedNumber, r.IsCheckedIn, r.Player == null ? null : new RegistrationPlayerBriefDto(r.Player.Id, r.Player.FirstName, r.Player.LastName))).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<RegistrationDto>> GetOne(Guid id)
    {
        var r = await _db.Registrations.Include(x => x.Player).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (r == null) return NotFound();
        return Ok(new RegistrationDto(r.Id, r.PlayerId, r.TournamentId, r.RegisteredUtc, r.SeedNumber, r.IsCheckedIn, r.Player == null ? null : new RegistrationPlayerBriefDto(r.Player.Id, r.Player.FirstName, r.Player.LastName)));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RegistrationDto>> Post([FromBody] RegistrationCreateDto dto)
    {
        var entity = new Registration
        {
            Id = Guid.NewGuid(),
            PlayerId = dto.PlayerId,
            TournamentId = dto.TournamentId,
            RegisteredUtc = DateTime.SpecifyKind(dto.RegisteredUtc, DateTimeKind.Utc),
            SeedNumber = dto.SeedNumber,
            IsCheckedIn = dto.IsCheckedIn
        };
        _db.Registrations.Add(entity);
        await _db.SaveChangesAsync();
        var created = await _db.Registrations.Include(x => x.Player).AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
        var result = new RegistrationDto(created.Id, created.PlayerId, created.TournamentId, created.RegisteredUtc, created.SeedNumber, created.IsCheckedIn, created.Player == null ? null : new RegistrationPlayerBriefDto(created.Player.Id, created.Player.FirstName, created.Player.LastName));
        return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(Guid id, [FromBody] RegistrationUpdateDto dto)
    {
        var entity = await _db.Registrations.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();
        entity.PlayerId = dto.PlayerId;
        entity.TournamentId = dto.TournamentId;
        entity.RegisteredUtc = DateTime.SpecifyKind(dto.RegisteredUtc, DateTimeKind.Utc);
        entity.SeedNumber = dto.SeedNumber;
        entity.IsCheckedIn = dto.IsCheckedIn;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _db.Registrations.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();
        _db.Registrations.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
