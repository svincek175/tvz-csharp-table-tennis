namespace TableTennisTracker.Web.ViewModels;

public sealed class AutocompleteLookupViewModel
{
    public string InputId { get; init; } = string.Empty;
    public string InputName { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string Placeholder { get; init; } = string.Empty;
    public string LookupUrl { get; init; } = string.Empty;
    public string? SelectedValue { get; init; }
    public string? SelectedText { get; init; }
    public string? HelpText { get; init; }
}
