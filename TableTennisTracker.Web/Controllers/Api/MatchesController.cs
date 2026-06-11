using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TableTennisTracker.Web.Infrastructure;
using TableTennisTracker.Web.ViewModels.Api;
using TableTennisTracker.Domain.Models;

namespace TableTennisTracker.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly TableTennisTrackerDbContext _db;

    public MatchesController(TableTennisTrackerDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MatchDto>>> Get([FromQuery] string? query)
    {
        var q = _db.Matches.Include(m => m.Participants).Include(m => m.SetResults).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var lower = query.Trim().ToLower();
            // allow search by tournament name via navigation
        }

        var list = await q.Select(m => new MatchDto(m.Id, m.TournamentId, m.RoundNumber, m.TableNumber, m.ScheduledStartUtc, m.ActualStartUtc, m.CompletedUtc, m.BestOfSets, m.WinnerPlayerId, m.Status.ToString(), m.Participants.Select(p => new MatchParticipantDto(p.Id, p.MatchId, p.PlayerId, p.Slot, p.ScoreSetsWon, null)), m.SetResults.Select(s => new MatchSetResultDto(s.Id, s.MatchId, s.SetNumber, s.PlayerOnePoints, s.PlayerTwoPoints)))).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<MatchDto>> GetOne(Guid id)
    {
        var m = await _db.Matches.Include(x => x.Participants).Include(x => x.SetResults).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (m == null) return NotFound();
        var dto = new MatchDto(m.Id, m.TournamentId, m.RoundNumber, m.TableNumber, m.ScheduledStartUtc, m.ActualStartUtc, m.CompletedUtc, m.BestOfSets, m.WinnerPlayerId, m.Status.ToString(), m.Participants.Select(p => new MatchParticipantDto(p.Id, p.MatchId, p.PlayerId, p.Slot, p.ScoreSetsWon, null)), m.SetResults.Select(s => new MatchSetResultDto(s.Id, s.MatchId, s.SetNumber, s.PlayerOnePoints, s.PlayerTwoPoints)));
        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MatchDto>> Post([FromBody] MatchCreateDto dto)
    {
        var entity = new Match
        {
            Id = Guid.NewGuid(),
            TournamentId = dto.TournamentId,
            RoundNumber = dto.RoundNumber,
            TableNumber = dto.TableNumber,
            ScheduledStartUtc = DateTime.SpecifyKind(dto.ScheduledStartUtc, DateTimeKind.Utc),
            ActualStartUtc = dto.ActualStartUtc == null ? null : DateTime.SpecifyKind(dto.ActualStartUtc.Value, DateTimeKind.Utc),
            CompletedUtc = dto.CompletedUtc == null ? null : DateTime.SpecifyKind(dto.CompletedUtc.Value, DateTimeKind.Utc),
            BestOfSets = dto.BestOfSets,
            WinnerPlayerId = dto.WinnerPlayerId,
            Status = Enum.Parse<TableTennisTracker.Domain.Enums.MatchStatus>(dto.Status)
        };
        _db.Matches.Add(entity);
        await _db.SaveChangesAsync();
        var created = await _db.Matches.Include(x => x.Participants).Include(x => x.SetResults).AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
        var result = new MatchDto(created.Id, created.TournamentId, created.RoundNumber, created.TableNumber, created.ScheduledStartUtc, created.ActualStartUtc, created.CompletedUtc, created.BestOfSets, created.WinnerPlayerId, created.Status.ToString(), created.Participants.Select(p => new MatchParticipantDto(p.Id, p.MatchId, p.PlayerId, p.Slot, p.ScoreSetsWon, null)), created.SetResults.Select(s => new MatchSetResultDto(s.Id, s.MatchId, s.SetNumber, s.PlayerOnePoints, s.PlayerTwoPoints)));
        return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(Guid id, [FromBody] MatchUpdateDto dto)
    {
        var entity = await _db.Matches.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();
        entity.TournamentId = dto.TournamentId;
        entity.RoundNumber = dto.RoundNumber;
        entity.TableNumber = dto.TableNumber;
        entity.ScheduledStartUtc = DateTime.SpecifyKind(dto.ScheduledStartUtc, DateTimeKind.Utc);
        entity.ActualStartUtc = dto.ActualStartUtc == null ? null : DateTime.SpecifyKind(dto.ActualStartUtc.Value, DateTimeKind.Utc);
        entity.CompletedUtc = dto.CompletedUtc == null ? null : DateTime.SpecifyKind(dto.CompletedUtc.Value, DateTimeKind.Utc);
        entity.BestOfSets = dto.BestOfSets;
        entity.WinnerPlayerId = dto.WinnerPlayerId;
        entity.Status = Enum.Parse<TableTennisTracker.Domain.Enums.MatchStatus>(dto.Status);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _db.Matches.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();
        _db.Matches.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
