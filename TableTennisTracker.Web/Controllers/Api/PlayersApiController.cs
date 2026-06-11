using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TableTennisTracker.Web.Infrastructure;
using TableTennisTracker.Web.ViewModels.Api;
using TableTennisTracker.Domain.Models;

namespace TableTennisTracker.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly TableTennisTrackerDbContext _db;

    public PlayersController(TableTennisTrackerDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> Get([FromQuery] string? query)
    {
        var q = _db.Players.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var lower = query.Trim().ToLower();
            q = q.Where(p => p.FirstName.ToLower().Contains(lower) || p.LastName.ToLower().Contains(lower));
        }

        var list = await q.Select(p => new PlayerDto(p.Id, p.FirstName, p.LastName, p.DateOfBirth, p.CountryCode, p.CurrentRankingPoints, p.IsActive)).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PlayerDto>> GetOne(Guid id)
    {
        var p = await _db.Players.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return NotFound();
        return Ok(new PlayerDto(p.Id, p.FirstName, p.LastName, p.DateOfBirth, p.CountryCode, p.CurrentRankingPoints, p.IsActive));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PlayerDto>> Post([FromBody] PlayerCreateDto dto)
    {
        var entity = new Player
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            CountryCode = dto.CountryCode,
            CurrentRankingPoints = dto.CurrentRankingPoints,
            IsActive = dto.IsActive,
            CreatedUtc = DateTime.UtcNow
        };

        _db.Players.Add(entity);
        await _db.SaveChangesAsync();

        var result = new PlayerDto(entity.Id, entity.FirstName, entity.LastName, entity.DateOfBirth, entity.CountryCode, entity.CurrentRankingPoints, entity.IsActive);
        return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(Guid id, [FromBody] PlayerUpdateDto dto)
    {
        var entity = await _db.Players.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.DateOfBirth = dto.DateOfBirth;
        entity.CountryCode = dto.CountryCode;
        entity.CurrentRankingPoints = dto.CurrentRankingPoints;
        entity.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _db.Players.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();
        _db.Players.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
