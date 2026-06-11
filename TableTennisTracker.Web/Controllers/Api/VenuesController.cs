using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TableTennisTracker.Web.Infrastructure;
using TableTennisTracker.Web.ViewModels.Api;
using TableTennisTracker.Domain.Models;

namespace TableTennisTracker.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly TableTennisTrackerDbContext _db;

    public VenuesController(TableTennisTrackerDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<VenueDto>>> Get([FromQuery] string? query)
    {
        var q = _db.Venues.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var lower = query.Trim().ToLower();
            q = q.Where(v => v.Name.ToLower().Contains(lower) || v.City.ToLower().Contains(lower));
        }

        var list = await q.Select(v => new VenueDto(v.Id, v.Name, v.City, v.CountryCode, v.AddressLine, v.NumberOfTables, v.Capacity, v.TimeZoneId)).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<VenueDto>> GetOne(Guid id)
    {
        var v = await _db.Venues.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (v == null) return NotFound();
        return Ok(new VenueDto(v.Id, v.Name, v.City, v.CountryCode, v.AddressLine, v.NumberOfTables, v.Capacity, v.TimeZoneId));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<VenueDto>> Post([FromBody] VenueCreateDto dto)
    {
        var entity = new Venue
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            City = dto.City,
            CountryCode = dto.CountryCode,
            AddressLine = dto.AddressLine,
            NumberOfTables = dto.NumberOfTables,
            Capacity = dto.Capacity,
            TimeZoneId = dto.TimeZoneId
        };
        _db.Venues.Add(entity);
        await _db.SaveChangesAsync();
        var result = new VenueDto(entity.Id, entity.Name, entity.City, entity.CountryCode, entity.AddressLine, entity.NumberOfTables, entity.Capacity, entity.TimeZoneId);
        return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(Guid id, [FromBody] VenueUpdateDto dto)
    {
        var entity = await _db.Venues.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();
        entity.Name = dto.Name;
        entity.City = dto.City;
        entity.CountryCode = dto.CountryCode;
        entity.AddressLine = dto.AddressLine;
        entity.NumberOfTables = dto.NumberOfTables;
        entity.Capacity = dto.Capacity;
        entity.TimeZoneId = dto.TimeZoneId;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _db.Venues.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();
        _db.Venues.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
