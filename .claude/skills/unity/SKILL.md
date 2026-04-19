---
name: unity
description: Launch Unity game builder by genre. Usage: /unity [genre] [args] or /unity setup
---

BEFORE ANYTHING ELSE: You MUST use the Skill tool to invoke "unity-mcp-skill". This loads the unity-mcp tool schemas, best practices, and workflow patterns that all unity skills depend on. Do not proceed without loading it first.

Available genres: slot
Utility commands: setup

Dispatch rules:

1. If "$0" is "setup" or "install", use the Skill tool to invoke "unity-setup".
2. If "$0" matches a genre or prefix, resolve to the full genre name, then use the Skill tool to invoke "unity-$0" with remaining args.
3. If "$0" is empty or not in the genre/utility list, list available options and ask which one to run.
4. Pass all additional arguments as context for the invoked skill.

Genre matching:
- slot, slot-machine, slots → `unity-slot`

Utility matching:
- setup, install, configure → `unity-setup`

ARGUMENTS: $ARGUMENTS