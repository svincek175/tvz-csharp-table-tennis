namespace TableTennisTracker.Web.ViewModels;

public sealed class DatePickerViewModel
{
    public string FieldId { get; init; } = string.Empty;
    public string FieldName { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string? Value { get; init; }
    public bool EnableTime { get; init; }
}
