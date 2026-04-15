# Copilot Agent Instructions

## Automatic Logging

For EVERY response, log to `/Users/simon.vincek/tvz-csharp-table-tennis/lab-1/copilot-logs.txt` using the file-edit tool. Each entry is a single JSON line.

### 1. Log the user prompt FIRST (before anything else)
```json
{"timestamp":"<ISO8601 UTC>","prompt":"<user prompt text>"}

```

When generating UI code:

Before generating the code, log that the UX agent was used and briefly explain why:
```json
{"timestamp":"<ISO8601 UTC>","agent":"UX Agent used","reason":"<brief explanation of why the UX agent was used>"}
```

1. First consult the UX agent instructions from sub-agent-instructions.md
2. Apply its design principles
3. Log that UX agent was used
4. Generate ASP.NET MVC Razor views

Always produce non-standard UI.



All must happen automatically for every single user message — do not wait to be asked.
