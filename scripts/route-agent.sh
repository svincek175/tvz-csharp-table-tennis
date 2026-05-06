#!/bin/bash

INPUT=$(cat)

PROMPT=$(echo "$INPUT" | tr '[:upper:]' '[:lower:]')

echo "ROUTING DECISION: $PROMPT" >> ai_logs.txt

if [[ "$PROMPT" == *"ui"* || "$PROMPT" == *"ux"* || "$PROMPT" == *"design"* ]]; then
    echo "SELECTED AGENT: UX-AGENT" >> ai_logs.txt
elif [[ "$PROMPT" == *"api"* || "$PROMPT" == *"controller"* || "$PROMPT" == *"database"* ]]; then
    echo "SELECTED AGENT: BACKEND-AGENT" >> ai_logs.txt
else
    echo "SELECTED AGENT: DEFAULT" >> ai_logs.txt
fi