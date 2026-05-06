#!/bin/bash

INPUT=$(cat)

echo "====================" >> ai_logs.txt
echo "PROMPT EVENT:" >> ai_logs.txt
echo "$INPUT" >> ai_logs.txt
echo "TIME: $(date)" >> ai_logs.txt
echo "====================" >> ai_logs.txt