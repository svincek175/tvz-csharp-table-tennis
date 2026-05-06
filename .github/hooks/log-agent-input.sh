#!/bin/bash

EVENT_NAME=$1
INPUT=$(cat)

LOG_FILE="copilot-logs.txt"

echo "=========================" >> "$LOG_FILE"
echo "EVENT: $EVENT_NAME" >> "$LOG_FILE"
echo "TIME: $(date)" >> "$LOG_FILE"
echo "INPUT:" >> "$LOG_FILE"
echo "$INPUT" >> "$LOG_FILE"
echo "=========================" >> "$LOG_FILE"
echo "" >> "$LOG_FILE"

exit 0