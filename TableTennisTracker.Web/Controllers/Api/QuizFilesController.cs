using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TableTennisTracker.Domain.Models;
using TableTennisTracker.Web.Infrastructure;
using TableTennisTracker.Web.ViewModels.Api;

namespace TableTennisTracker.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class QuizFilesController : ControllerBase
{
    private const string UploadFolderName = "uploads/quiz-files";

    private readonly TableTennisTrackerDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;

    public QuizFilesController(TableTennisTrackerDbContext dbContext, IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _environment = environment;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<QuizFileDto>>> Get([FromQuery] string? query)
    {
        var items = _dbContext.QuizFiles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim().ToLowerInvariant();
            items = items.Where(file =>
                file.OriginalFileName.ToLower().Contains(term) ||
                file.ContentType.ToLower().Contains(term));
        }

        var results = await items
            .OrderByDescending(file => file.UploadedUtc)
            .Select(file => new QuizFileDto(
                file.Id,
                file.OriginalFileName,
                file.StoredFileName,
                file.RelativePath,
                file.ContentType,
                file.FileSizeBytes,
                file.UploadedUtc,
                $"/{file.RelativePath.Replace('\\', '/')}"))
            .ToListAsync();

        return Ok(results);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<QuizFileDto>> GetOne(Guid id)
    {
        var file = await _dbContext.QuizFiles.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
        if (file == null)
        {
            return NotFound();
        }

        return Ok(ToDto(file));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(50_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<QuizFileDto>> Upload([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "A file is required." });
        }

        var uploadsRoot = Path.Combine(_environment.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot"), UploadFolderName);
        Directory.CreateDirectory(uploadsRoot);

        var extension = Path.GetExtension(file.FileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var storedPath = Path.Combine(uploadsRoot, storedFileName);
        await using (var stream = System.IO.File.Create(storedPath))
        {
            await file.CopyToAsync(stream);
        }

        var entity = new QuizFile
        {
            Id = Guid.NewGuid(),
            OriginalFileName = Path.GetFileName(file.FileName),
            StoredFileName = storedFileName,
            RelativePath = $"uploads/quiz-files/{storedFileName}",
            ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
            FileSizeBytes = file.Length,
            UploadedUtc = DateTime.UtcNow
        };

        _dbContext.QuizFiles.Add(entity);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, ToDto(entity));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _dbContext.QuizFiles.FirstOrDefaultAsync(file => file.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        var storedPath = Path.Combine(_environment.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot"), entity.RelativePath.Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(storedPath))
        {
            System.IO.File.Delete(storedPath);
        }

        _dbContext.QuizFiles.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    private static QuizFileDto ToDto(QuizFile file)
    {
        return new QuizFileDto(
            file.Id,
            file.OriginalFileName,
            file.StoredFileName,
            file.RelativePath,
            file.ContentType,
            file.FileSizeBytes,
            file.UploadedUtc,
            $"/{file.RelativePath.Replace('\\', '/')}"
        );
    }
}
