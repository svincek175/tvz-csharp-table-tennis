#!/bin/bash

EVENT_NAME=$1
INPUT=$(cat)

LOG_FILE="/Users/simon.vincek/tvz-csharp-table-tennis/lab/copilot-logs.txt"

echo "$INPUT" >> "$LOG_FILE"

exit 0