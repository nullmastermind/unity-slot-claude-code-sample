---
name: unity-slot
description: Plan and build a slot machine game in Unity. Explore game design, then implement via subagents.
---

You are planning a slot machine game in Unity.

BEFORE PROCEEDING: You MUST use the Skill tool to invoke "unity-explore". This loads the shared explore mode behavior (Feynman technique, visualization, continuous verification, subagent protocols, implementation options) that this command depends on. Do not proceed without loading it first.

---

## What You Might Do

**Explore the game concept**
- Feynman Echo: "So you want a game where the player presses Spin, reels spin and stop randomly, matching symbols on lines pay credits. Is that right?"
- What makes THIS slot game different from a generic one?
- What's the theme? (casino classic, fantasy, space, fruit...)
- What's the target feel? (casual mobile, flashy casino, retro arcade)

**Discuss core mechanics**
- Reel configuration: how many reels, how many rows?
- Symbol set: how many symbols, tiers (low/mid/high/special)?
- Paylines: fixed or selectable? how many?
- Betting: min/max, increments?
- Bonus features: free spins? bonus rounds? multipliers? progressive jackpot?
- Win math: how is payout calculated?

**Visualize**
```
Slot game ASCII layout example:

┌────────────────────────────────────┐
│  Balance: 1000    Win: ---   Bet: 5│
├────────────────────────────────────┤
│  ┌────┐ ┌────┐ ┌────┐ ┌────┐ ┌────┐│
│  │ 🍒 │ │ 🍋 │ │ 7  │ │ 🍒 │ │ BAR││
│  ├────┤ ├────┤ ├────┤ ├────┤ ├────┤│
│  │ 7  │ │ 🍒 │ │ 🍒 │ │ 🔔 │ │ 🍇 ││
│  ├────┤ ├────┤ ├────┤ ├────┤ ├────┤│
│  │ BAR│ │ 🔔 │ │ 🍇 │ │ 7  │ │ 🍒 ││
│  └────┘ └────┘ └────┘ └────┘ └────┘│
├────────────────────────────────────┤
│  [-] Bet: 5 [+] [MAX]   [SPIN]    │
└────────────────────────────────────┘

State machine:
IDLE → SPINNING → STOPPING → EVALUATING → PAYING → IDLE
                                            ↓
                                       FREE_SPINS
```

**Discuss UI screens**
- What screens does the game need? (menu, game, settings, paytable?)
- HUD layout: where does balance, bet, win display go?
- What popups? (win celebration, free spins trigger, insufficient balance)
- Paytable: embedded or overlay?

**Discuss visual style**
- 2D or 3D reels?
- Color palette and mood
- VFX: particle effects for wins? celebrations?
- Lighting and post-processing

**Research if needed**
- Slot game math models and RNG patterns
- Unity reel animation techniques (DOTween, AnimationCurve, coroutine-based)
- Delegate to osf-researcher for web research

---

## Stress-test Questions (Slot-specific)

Resolve these before ending discovery. Self-answer where possible, only surface genuinely ambiguous items:

1. Reel mechanics:
   "How do reels stop?"
   A. All at once
   B. ★ Cascade left to right with delay
   C. Random order
   D. Other: ___

2. Win evaluation:
   "How are wins detected?"
   A. Center line only (1 payline)
   B. Multiple fixed paylines (5/10/20/25)
   C. ★ 20 configurable paylines, left-to-right, minimum 3 matching
   D. Ways-to-win (243 ways)
   E. Other: ___

3. Special symbols:
   "Which special symbols?"
   A. None (pure matching)
   B. Wild only
   C. ★ Wild + Scatter (scatter triggers free spins)
   D. Wild + Scatter + Bonus symbol
   E. Other: ___

4. Progression:
   "How does the player progress?"
   A. ★ Session-based (play until credits run out or quit)
   B. Level-based (unlock new themes/features)
   C. Achievement-based
   D. Other: ___

5. Data persistence:
   "What saves between sessions?"
   A. ★ Nothing (fresh 1000 credits each launch)
   B. Balance persists (PlayerPrefs)
   C. Full save (balance + unlocks + settings)
   D. Other: ___

6. Animation approach:
   "Reel animation method:"
   A. Unity Animation system
   B. ★ Code-driven (coroutine + AnimationCurve/easing)
   C. DOTween plugin
   D. Other: ___

---

## Zero-Fog Checklist (additions)

- [ ] Reel configuration is specific (N reels x M rows, exact numbers)
- [ ] Symbol set is defined (exact symbols, tiers, special behaviors)
- [ ] Payline count and pattern is decided
- [ ] Bet range and increments are defined
- [ ] Win calculation formula is explicit
- [ ] Bonus features are specific (trigger condition, reward, duration)
- [ ] Every UI screen has concrete elements listed
- [ ] State machine transitions are complete (including edge cases: insufficient balance, free spins retrigger)

---

The following is the user's request: