using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TableTennisTracker.Web.Infrastructure;
using TableTennisTracker.Web.ViewModels.Api;
using TableTennisTracker.Domain.Models;

namespace TableTennisTracker.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class MatchSetResultsController : ControllerBase
{
    private readonly TableTennisTrackerDbContext _db;

    public MatchSetResultsController(TableTennisTrackerDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MatchSetResultDto>>> Get([FromQuery] Guid? matchId)
    {
        var q = _db.MatchSetResults.AsNoTracking().AsQueryable();
        if (matchId.HasValue) q = q.Where(x => x.MatchId == matchId.Value);
        var list = await q.Select(s => new MatchSetResultDto(s.Id, s.MatchId, s.SetNumber, s.PlayerOnePoints, s.PlayerTwoPoints)).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<MatchSetResultDto>> GetOne(Guid id)
    {
        var s = await _db.MatchSetResults.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return NotFound();
        return Ok(new MatchSetResultDto(s.Id, s.MatchId, s.SetNumber, s.PlayerOnePoints, s.PlayerTwoPoints));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MatchSetResultDto>> Post([FromBody] MatchSetResultCreateDto dto)
    {
        var entity = new MatchSetResult
        {
            Id = Guid.NewGuid(),
            MatchId = dto.MatchId,
            SetNumber = dto.SetNumber,
            PlayerOnePoints = dto.PlayerOnePoints,
            PlayerTwoPoints = dto.PlayerTwoPoints
        };
        _db.MatchSetResults.Add(entity);
        await _db.SaveChangesAsync();
        var created = await _db.MatchSetResults.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
        return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, new MatchSetResultDto(created.Id, created.MatchId, created.SetNumber, created.PlayerOnePoints, created.PlayerTwoPoints));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(Guid id, [FromBody] MatchSetResultUpdateDto dto)
    {
        var entity = await _db.MatchSetResults.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();
        entity.MatchId = dto.MatchId;
        entity.SetNumber = dto.SetNumber;
        entity.PlayerOnePoints = dto.PlayerOnePoints;
        entity.PlayerTwoPoints = dto.PlayerTwoPoints;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _db.MatchSetResults.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();
        _db.MatchSetResults.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
