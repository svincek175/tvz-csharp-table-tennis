using System;

namespace TableTennisTracker.Web.ViewModels.Api;

public record QuizFileDto(
    Guid Id,
    string OriginalFileName,
    string StoredFileName,
    string RelativePath,
    string ContentType,
    long FileSizeBytes,
    DateTime UploadedUtc,
    string DownloadUrl);
