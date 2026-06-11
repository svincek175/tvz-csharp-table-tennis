using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TableTennisTracker.Web.Infrastructure;
using TableTennisTracker.Web.ViewModels.Api;
using TableTennisTracker.Domain.Models;

namespace TableTennisTracker.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class MatchParticipantsController : ControllerBase
{
    private readonly TableTennisTrackerDbContext _db;

    public MatchParticipantsController(TableTennisTrackerDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MatchParticipantDto>>> Get([FromQuery] Guid? matchId)
    {
        var q = _db.MatchParticipants.Include(mp => mp.Player).AsNoTracking().AsQueryable();
        if (matchId.HasValue) q = q.Where(x => x.MatchId == matchId.Value);

        var list = await q.Select(mp => new MatchParticipantDto(mp.Id, mp.MatchId, mp.PlayerId, mp.Slot, mp.ScoreSetsWon, mp.Player == null ? null : new MatchParticipantPlayerBriefDto(mp.Player.Id, mp.Player.FirstName, mp.Player.LastName))).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<MatchParticipantDto>> GetOne(Guid id)
    {
        var mp = await _db.MatchParticipants.Include(x => x.Player).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (mp == null) return NotFound();
        return Ok(new MatchParticipantDto(mp.Id, mp.MatchId, mp.PlayerId, mp.Slot, mp.ScoreSetsWon, mp.Player == null ? null : new MatchParticipantPlayerBriefDto(mp.Player.Id, mp.Player.FirstName, mp.Player.LastName)));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MatchParticipantDto>> Post([FromBody] MatchParticipantCreateDto dto)
    {
        var entity = new MatchParticipant
        {
            Id = Guid.NewGuid(),
            MatchId = dto.MatchId,
            PlayerId = dto.PlayerId,
            Slot = dto.Slot,
            ScoreSetsWon = dto.ScoreSetsWon
        };
        _db.MatchParticipants.Add(entity);
        await _db.SaveChangesAsync();
        var created = await _db.MatchParticipants.Include(x => x.Player).AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
        var result = new MatchParticipantDto(created.Id, created.MatchId, created.PlayerId, created.Slot, created.ScoreSetsWon, created.Player == null ? null : new MatchParticipantPlayerBriefDto(created.Player.Id, created.Player.FirstName, created.Player.LastName));
        return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(Guid id, [FromBody] MatchParticipantUpdateDto dto)
    {
        var entity = await _db.MatchParticipants.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();
        entity.MatchId = dto.MatchId;
        entity.PlayerId = dto.PlayerId;
        entity.Slot = dto.Slot;
        entity.ScoreSetsWon = dto.ScoreSetsWon;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _db.MatchParticipants.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();
        _db.MatchParticipants.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
