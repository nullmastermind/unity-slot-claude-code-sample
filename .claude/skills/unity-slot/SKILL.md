---
name: unity-slot
description: Build a slot machine game from scratch in Unity via unity-mcp.
---

You are building a slot machine game in Unity using unity-mcp MCP tools.

BEFORE PROCEEDING: Use the Skill tool to invoke "unity-pipeline". This loads the shared build pipeline that orchestrates all phase skills. Do not proceed without loading it first.

---

GAME BLUEPRINT

Project: SlotGame

Scenes:
- MainMenu — title screen, play/settings/quit
- SlotGame — primary gameplay scene
- Settings — audio and graphics settings

Core Systems:

1. Reel System
   - 5 reels, 3 visible rows each
   - Configurable spin speed and stop delay per reel (cascade stop left to right)
   - Spin animation with easing (ease-out bounce on stop)

2. Symbols
   - Low tier: Cherry, Lemon, Orange, Grape
   - Mid tier: Bell, Bar
   - High tier: Seven
   - Special: Wild (substitutes any except Scatter), Scatter (pays anywhere)

3. Paylines
   - 20 configurable paylines
   - Payline patterns stored in ScriptableObject
   - Visual indicator on reel grid edges showing active paylines

4. Betting
   - Min bet: 1 credit
   - Max bet: 100 credits
   - Quick buttons: +1, +5, +10, Max Bet
   - Bet per line × active paylines = total bet

5. Win Detection
   - Left-to-right matching on each payline (minimum 3 matching)
   - Wild substitution (not for Scatter)
   - Scatter pays: 3+ anywhere triggers Free Spins
   - Win amount = symbol payout × bet per line × match count multiplier

6. Bonus Features
   - Free Spins: 3+ Scatter → 10 free spins, retriggerable
   - Big Win celebration: wins above 10× bet get special animation
   - Mega Win: wins above 25× bet get escalated celebration

7. Balance
   - Starting credits: 1000
   - Persistent within session
   - Insufficient balance → disable Spin button, show warning

Game State Machine:
```
IDLE → BETTING → SPINNING → STOPPING → EVALUATING → PAYING → IDLE
                                                        ↓
                                                   FREE_SPINS
                                                   (loop back to SPINNING)
```

---

Scripts:

Core:
- GameManager.cs — game state machine, scene transitions, session data
- SlotMachine.cs — orchestrates spin cycle (start spin, stop reels, evaluate, pay)
- ReelController.cs — single reel: spin, stop with easing, symbol positioning
- ReelStrip.cs — defines symbol sequence for a reel (ScriptableObject)
- SymbolDatabase.cs — ScriptableObject with symbol definitions, sprites, payout table
- PaylineDefinition.cs — ScriptableObject defining payline patterns
- PaylineEvaluator.cs — check all paylines for wins, calculate payout
- BetController.cs — manage bet amount, bet per line, active paylines
- BalanceManager.cs — track credits, add/subtract, insufficient balance check

Bonus:
- BonusManager.cs — free spins logic, bonus tracking, retrigger

UI:
- UIManager.cs — screen transitions, popup management
- SpinButton.cs — spin/stop button behavior, auto-spin toggle
- WinDisplay.cs — animated credit counter, win tier detection (Win/BigWin/MegaWin)

Audio:
- AudioManager.cs — play SFX and music, volume control, sound pooling
- Sounds: reel_spin, reel_stop, win_small, win_big, win_mega, button_click, coin_credit

---

UI Blueprint:

Main Menu:
- Title text: game name, large, centered
- Play button: prominent, center
- Settings button: below play
- Quit button: bottom
- Background: dark gradient with subtle particle ambiance

Slot HUD (primary game screen):
- Layout top to bottom:
  1. Top bar: Balance (left), Win amount (center), Bet display (right)
  2. Reel area: 5×3 grid with decorative frame
     - Payline indicators on left/right edges
     - Win highlight overlay on matching symbols
  3. Bottom controls:
     - Bet adjustment: [-] [amount] [+] [MAX BET]
     - Spin button: large, center, changes to STOP during spin
     - Auto-spin toggle with spin count selector
  4. Info bar: paytable button, paylines button

Paytable (overlay panel):
- Tab 1: Symbol payouts — icon + payout for 3/4/5 match
- Tab 2: Payline diagram — visual grid showing all 20 lines
- Tab 3: Rules — wild, scatter, free spins explanation
- Close button top-right

Settings:
- Music volume slider
- SFX volume slider
- Graphics quality dropdown (Low/Medium/High)
- Back button

Popups:
- Win popup: animated credit count-up, scales with win tier
- Free Spins trigger: "FREE SPINS WON!" with scatter animation
- Free Spins HUD: remaining spins counter replaces bet controls
- Insufficient balance warning

---

Assets:

Materials:
- Reel background: dark semi-transparent
- Reel frame: metallic gold/bronze
- Symbol highlight: glowing emission material for wins
- Button materials: normal, hover, pressed states

VFX:
- Win particle burst on matching symbols
- Big Win screen-wide particle celebration
- Coin shower effect for Mega Win
- Subtle ambient particles on main menu

Lighting:
- Ambient: warm, casino-themed
- Accent lights on reel frame
- Post-processing: bloom for win effects, vignette

Camera:
- Orthographic for 2D slot layout
- Fixed position, no movement

---

Physics: not needed for this genre. Skip Phase 6.