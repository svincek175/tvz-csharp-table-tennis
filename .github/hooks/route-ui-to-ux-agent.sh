#!/bin/bash

EVENT_NAME=$1
INPUT=$(cat)

if ! command -v jq >/dev/null 2>&1; then
    echo "$INPUT"
    exit 0
fi

# Only transform user prompts.
if [ "$EVENT_NAME" != "UserPromptSubmit" ] && [ "$EVENT_NAME" != "userPromptSubmitted" ]; then
    echo "$INPUT"
    exit 0
fi

PROMPT=$(printf '%s' "$INPUT" | jq -r '.prompt // empty')

if [ -z "$PROMPT" ]; then
    echo "$INPUT"
    exit 0
fi

# Do not duplicate the directive if already requested.
if printf '%s' "$PROMPT" | grep -Eiq 'ux\s*agent|subagent'; then
    echo "$INPUT"
    exit 0
fi

# Route only UI/UX/front-end related prompts.
if printf '%s' "$PROMPT" | grep -Eiq '(^|[^a-z])(ui|ux|frontend|front-end|design|layout|view|views|cshtml|css|styling|style|responsive|navigation|breadcrumb|sidebar)([^a-z]|$)'; then
    DIRECTIVE='Use the subagent named "UX Agent" from .github/agents/ux-agent.agent.md for all UI/UX implementation in this request. Delegate UI changes to that subagent first, then apply results.'
    NEW_PROMPT="$DIRECTIVE

$PROMPT"
    printf '%s' "$INPUT" | jq --arg newPrompt "$NEW_PROMPT" '.prompt = $newPrompt'
    exit 0
fi

echo "$INPUT"
