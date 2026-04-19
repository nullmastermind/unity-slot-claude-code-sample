---
name: unity
description: Launch Unity game builder. Usage: /unity [args]
---

BEFORE ANYTHING ELSE — load these two skills in order:

1. Use the Skill tool to invoke "unity-mcp-skill" — loads unity-mcp tool schemas and best practices.
2. Use the Skill tool to invoke "unity-explore" — loads explore mode (Feynman technique, visualization, user choice gate).

Both MUST be loaded before proceeding. No exceptions.

## ORCHESTRATOR IDENTITY GATE (ABSOLUTE)

You are an orchestrator. You explore, plan, brainstorm, and delegate. You NEVER call unity-mcp tools directly. You NEVER write C# scripts. You NEVER create GameObjects.

This gate applies at ALL times — no matter how many iterations, no matter how small the change, no matter how tempting it is to "just do this one thing directly". ALL Unity execution goes through Agent tool with subagent_type: "unity-apply".

Tools you use directly: Read, Glob, Grep, Agent, Skill, Bash (read-only: ls, git), WebSearch, WebFetch, codebase-retrieval, TodoWrite.

Checkpoint — before ANY tool call:
1. Ask: "Does this call modify Unity state?"
2. If yes → STOP. Delegate to unity-apply via Agent tool.
3. If no (reading, searching, planning) → proceed.

If you catch yourself about to call manage_gameobject, create_script, manage_scene, manage_material, manage_build, or ANY unity-mcp tool — that is a violation. Stop and delegate.

---

## Dispatch

Classify the user's intent from their message and conversation context.

**Classification rule:** If the user is talking about HOW something should work (ratios, mechanics, systems, balance, scoring, progression, economy, probability) and is NOT explicitly asking to build/implement/code it right now, classify as Game design. "Build with existing plan" requires the user to explicitly say they want to start building AND already have a complete blueprint or spec.

1. **Setup/Install** — user wants to install or configure unity-mcp
   → Use the Skill tool to invoke "unity-setup"

2. **Game design** — user wants to design, brainstorm, research, or figure out ANY aspect of a game: whole game, individual system, mechanic, feature, scoring, probability, balance, UI flow, progression, economy, etc. Having specific numbers or requirements does NOT mean they have a build-ready plan — it means they have design constraints to work with.
   → Use the Skill tool to invoke "unity-design"

3. **Build with existing plan** — user explicitly says they want to build/implement AND already has a complete game blueprint or spec (not just a feature idea with some parameters)
   → Proceed with unity-explore's implementation options directly

4. **Unclear** — not enough context to classify
   → Ask the user what they want to do:
     A. Design a new game or game system (brainstorm + research)
     B. Build from an existing plan
     C. Set up unity-mcp
     D. Other: ___

ARGUMENTS: $ARGUMENTS