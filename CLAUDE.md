# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

When asked about the codebase, project structure, or to find code, always use the augment-context-engine MCP tool (codebase-retrieval) in the root workspace first before reading individual files.

## Project Overview

A Unity 6 slot machine game with a 3-scene flow: MainMenu → DiceRoll → SlotGame. Built with URP and legacy UGUI. All visuals are procedurally generated at runtime using primitives and Unicode characters — no external art assets.

Unity version: 6000.3.13f1, URP 17.3.0.

## Architecture

```
MainMenu scene → MainMenuManager (button loads DiceRoll)
DiceRoll scene → DiceRoller (procedural 3D dice, physics roll, reads top face)
               → DiceCameraFollow (vertical camera follow when dice bounces high)
SlotGame scene → SlotGameManager (3-reel slot machine with weighted symbols)
GameData       → static class, cross-scene data bus (StartingPoints field)
```

All scripts are in `Assets/Scripts/` (5 files, flat structure). No ScriptableObjects, no prefabs, no singletons. Scene communication is purely `GameData` static fields + `SceneManager.LoadScene`.

Key design patterns:
- DiceRoller builds the entire 3D scene procedurally in `Start()` (ground, walls, dice with dot children, materials)
- SlotGameManager creates reel strips as UGUI Text elements, animates via coroutine (fast scroll → cubic ease-out → bounce overshoot)
- Symbol weights are hardcoded: `{26,23,20,18,10,7,4,2}` over 8 symbols (total 110)
- Face detection uses dot product of face normals against Vector3.up

## Build Configuration

Build scenes (order matters): MainMenu (0), DiceRoll (1), SlotGame (2).

No tests exist (test-framework package installed but unused). No CI pipeline.

## Unity-MCP Workflow

This project uses `/unity` skill as the main entry point for AI-assisted development:
- `/unity` routes to design, build, or setup flows
- `/unity-design` for game design brainstorm and research
- Build flow uses OpenSpec change tracking (`openspec/changes/`) with proposal → design → tasks → implementation
- All Unity Editor mutations go through the `unity-apply` subagent — the orchestrator never calls unity-mcp tools directly

## Key Dependencies

- `com.coplaydev.unity-mcp` 9.6.6 (via OpenUPM) — MCP bridge for Claude Code
- `com.unity.render-pipelines.universal` 17.3.0
- `com.unity.ugui` 2.0.0 (legacy Canvas/Text/Button — not TextMeshPro)
- No DOTween, no Cinemachine, no third-party animation libraries
