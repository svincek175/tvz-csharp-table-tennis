# Copilot Agent Instructions

## Automatic Logging

For EVERY response, log to `/Users/simon.vincek/tvz-csharp-table-tennis/lab-1/copilot-logs.txt` using the file-edit tool. Each entry is a single JSON line.

### 1. Log the user prompt FIRST (before anything else)
```json
{"timestamp":"<ISO8601 UTC>","prompt":"<user prompt text>"}

```

All must happen automatically for every single user message — do not wait to be asked. Use a consistent session_id (e.g. a short UUID) per conversation.
