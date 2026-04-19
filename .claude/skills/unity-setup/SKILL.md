---
name: unity-setup
description: Auto-install and configure unity-mcp from scratch. Run before first use of /unity.
---

You are setting up the unity-mcp bridge so that Claude Code can control Unity Editor via MCP tools. Follow these steps in order.

PREREQUISITES CHECK

1. Check Python version:
   Run: python --version (or python3 --version)
   Required: Python 3.10+
   If not installed, tell user to install from https://python.org

2. Check uv is installed:
   Run: uv --version
   If not installed, install it:
   - Windows: powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"
   - macOS/Linux: curl -LsSf https://astral.sh/uv/install.sh | sh

3. Check Unity Editor is installed:
   Ask user which Unity version they have (must be 2021.3 LTS or newer).
   Ask for the Unity project path they want to work with.

INSTALL UNITY PACKAGE

Guide user to install the MCP for Unity package in their Unity project:

Option A — Git URL (recommended):
1. Open Unity Editor
2. Go to Window > Package Manager
3. Click "+" button > "Add package from git URL..."
4. Paste: https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main
5. Click Add

Option B — OpenUPM:
Run in terminal: openupm add com.coplaydev.unity-mcp

Option C — Unity Asset Store:
Search "MCP for Unity" in Asset Store window inside Unity.

CONFIGURE MCP SERVER IN UNITY

After package is installed:
1. In Unity, go to Window > MCP for Unity
2. Click "Start Server"
3. Verify the server starts on localhost:8080
4. Look for the green "Connected" indicator

CONFIGURE CLAUDE CODE MCP CLIENT

Add unity-mcp to Claude Code's MCP settings.

Check if settings file exists:
- Global: ~/.claude/settings.json
- Project: .claude/settings.json

Add the following to the mcpServers section:

For HTTP transport (recommended, works on all platforms):
```json
{
  "mcpServers": {
    "unityMCP": {
      "url": "http://localhost:8080/mcp"
    }
  }
}
```

For stdio transport (alternative, macOS/Linux):
```json
{
  "mcpServers": {
    "unityMCP": {
      "command": "uvx",
      "args": ["--from", "mcpforunityserver", "mcp-for-unity", "--transport", "stdio"]
    }
  }
}
```

VERIFY CONNECTION

After configuration:
1. Restart Claude Code (or reload MCP servers)
2. Test the connection by calling the unity-mcp project_info resource
3. If successful, report:
   - Unity version
   - Project name
   - Render pipeline
   - Server URL

OPTIONAL: INSTALL ROSLYN VALIDATION

For strict C# script validation (catches undefined types, namespaces, methods):
1. In Unity, go to Window > MCP for Unity > Install Roslyn DLLs
2. Or manually:
   - Install NuGetForUnity package
   - Add Microsoft.CodeAnalysis v5.0
   - Add USE_ROSLYN to Project Settings > Player > Scripting Define Symbols

OPTIONAL: DISABLE TELEMETRY

unity-mcp collects anonymous usage data by default.
To opt out, set environment variable: DISABLE_TELEMETRY=true

TROUBLESHOOTING

Connection refused:
- Ensure Unity Editor is open and running
- Check Window > MCP for Unity shows "Server Running"
- Verify port 8080 is not blocked by firewall
- Try restarting the server in Unity

MCP tools not appearing:
- Restart Claude Code after adding mcpServers config
- Check settings.json syntax is valid JSON
- Verify the URL matches (http://localhost:8080/mcp)

Multiple Unity instances:
- Each instance runs on a different port
- Use set_active_instance tool with "Name@hash" to route to specific editor
- Check unity_instances resource for available editors

DONE

After setup is complete, announce:
```
## unity-mcp Setup Complete

Server: http://localhost:8080/mcp
Unity: [version]
Project: [name]
Pipeline: [URP/HDRP/Built-in]
Roslyn: [installed/not installed]

Ready to use /unity [genre] to build games.
```