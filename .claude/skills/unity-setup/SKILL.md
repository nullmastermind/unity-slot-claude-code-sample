---
name: unity-setup
description: Auto-install and configure unity-mcp from scratch. Run before first use of /unity.
---

You are setting up the unity-mcp bridge so that Claude Code can control Unity Editor via MCP tools. Automate every step possible — only ask the user when you genuinely need input.

STEP 1: PREREQUISITES

1. Check Python:
   Run: python --version (or python3 --version)
   Required: 3.10+
   If missing, install automatically:
   - Windows: winget install Python.Python.3.12
   - macOS: brew install python@3.12
   - Linux: sudo apt install python3.12 (or equivalent)

2. Check uv:
   Run: uv --version
   If missing, install automatically:
   - Windows: powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"
   - macOS/Linux: curl -LsSf https://astral.sh/uv/install.sh | sh

3. Check openupm-cli:
   Run: openupm --version
   If missing, install automatically:
   Run: npm install -g openupm-cli

4. Ask user for Unity project path (the folder containing Assets/).
   Verify the path exists and contains Assets/ folder.

STEP 2: INSTALL UNITY PACKAGE

Run in the Unity project directory:
```
openupm add com.coplaydev.unity-mcp
```

This adds the package to the project's manifest.json and scoped registry automatically. Unity will import it on next Editor focus.

After running, verify installation:
- Check that Packages/manifest.json contains "com.coplaydev.unity-mcp"
- Check that Packages/packages-lock.json lists the package

STEP 3: CONFIGURE CLAUDE CODE MCP CLIENT

Read the project's .claude/settings.json (or create it if it doesn't exist).
Add unity-mcp to the mcpServers section:

```json
{
  "mcpServers": {
    "unityMCP": {
      "url": "http://localhost:8080/mcp"
    }
  }
}
```

If settings.json already exists, merge the mcpServers entry — do not overwrite other settings.

STEP 4: START SERVER & VERIFY

Tell the user:
1. Open (or focus) Unity Editor — the project will auto-import the package
2. Go to Window > MCP for Unity
3. Click "Start Server"

Then test the connection by calling the unity-mcp project_info resource.
If connection fails, retry once after 5 seconds (Unity may still be importing).

STEP 5: OPTIONAL EXTRAS

Ask user if they want these (yes/no, default no):

Roslyn validation (strict C# checks):
  Tell user: In Unity, go to Window > MCP for Unity > Install Roslyn DLLs

Disable telemetry:
  Set DISABLE_TELEMETRY=true in environment.

TROUBLESHOOTING

openupm fails:
- Check npm is installed (node --version)
- If npm missing: install Node.js first (winget install OpenJS.NodeJS.LTS)
- Try: npx openupm-cli add com.coplaydev.unity-mcp (without global install)

Connection refused:
- Unity Editor must be open and focused (to trigger package import)
- Check Window > MCP for Unity shows "Server Running"
- Port 8080 may be in use — check Unity console for actual port

MCP tools not appearing after config:
- Restart Claude Code to reload MCP servers
- Verify .claude/settings.json syntax

DONE

After setup is complete, announce:
```
## unity-mcp Setup Complete

Server: http://localhost:8080/mcp
Unity: [version]
Project: [name]
Pipeline: [URP/HDRP/Built-in]

Ready to use /unity [genre] to build games.
```