# Copilot Agent Instructions

## Automatic Logging

For EVERY response, log to `/Users/simon.vincek/tvz-csharp-table-tennis/lab-1/copilot-logs.txt` using a file-edit action. Each entry is one JSON line.

### Required log sequence (strict order)

1. Log the user prompt FIRST (before anything else):
```json
{"timestamp":"<ISO8601 UTC>","prompt":"<user prompt text>"}
```

2. If the task includes UI generation or UI edits, BEFORE writing UI code:
   - Consult `.github/sub-agent-instructions.md`
   - Invoke UX sub-agent
   - Log UX usage:
```json
{"timestamp":"<ISO8601 UTC>","agent":"UX Agent used","reason":"<brief explanation>"}
```

3. Only after steps 1 and 2 may UI files be edited.

## UI generation rules

- Always produce non-standard UI.
- Generate ASP.NET MVC Razor views when UI is requested.
- For UI tasks, never skip UX sub-agent invocation + UX log line.

## Enforcement behavior

- If a UI task starts and no UX log entry exists for that request, STOP and add missing logs first.
- Do not continue to code until logs are present.
- This applies automatically for every user message.

## Definition of UI files

Treat these as UI work requiring UX logs:
- `TableTennisTracker.Web/Views/**`
- `TableTennisTracker.Web/wwwroot/**`
- `TableTennisTracker.Web/Controllers/**` when adding/changing UI workflow actions (Index/Details/Create/Edit/Delete navigation).
