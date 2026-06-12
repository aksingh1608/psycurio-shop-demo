# PsyCurio Shop Demo 🛒

A small interactive Unity shop scene built for the PsyCurio test task: click items on a shelf to place them on the counter, greet the shopkeeper, and check out at the cash register — all with a fixed camera and mouse clicks, playable entirely in the Unity editor.

![Demo](docs/demo.gif)

---

## Requirements & Tech Stack

| | |
|---|---|
| **Unity** | 6.3 LTS (6000.3.17f1) |
| **Render Pipeline** | Universal Render Pipeline (URP) |
| **Build Target** | Android (.apk), IL2CPP, ARM64 — build verified |
| **Input** | Unity Input System (new), mouse only |
| **Character & Animations** | Mixamo (idle, wave) |
| **UI** | TextMeshPro (world-space speech bubble + screen-space hints) |

The scene requires no XR packages and runs fully in the editor, as specified in the task.

## How to Run

1. Clone the repository and open the project in **Unity 6.3 LTS** (Unity Hub → Add → select the folder).
2. Open the scene: `Assets/Scenes/ShopScene.unity`.
3. Press **Play**.

### Controls
- **Left-click a shelf item** → a copy flies onto the counter (up to 5 items, duplicates allowed; prices are shown on the shelf tags)
- **Left-click the shopkeeper** → she waves at you
- **Left-click the cash register** → the shopkeeper summarizes your purchase and the total price in a speech bubble; a few seconds later the counter clears for the next customer

### Android Build
`File → Build Profiles → Android → Build`. The project is configured for IL2CPP / ARM64 with ShopScene in the scene list; a successful `.apk` build has been verified locally (interactions are mouse-based and editor-only, per the task brief).

## Implemented Features

**Core task**
- Shelf with multiple purchasable items; clicking places a copy on the counter (max 5)
- Shopkeeper (Mixamo character) idling beside the counter; waves when clicked
- Cash register click → speech bubble with itemized purchase summary (with quantity grouping, e.g. "2x Apple") and total price

**Additional task (chosen: effects)**
- Purchased items **fly from the shelf to the counter** along a parabolic arc with a spin
- **Particle burst** and **sound effects** (whoosh in flight, pop on landing)

**Extra UX features** (added because the brief emphasizes usability and user experience)
- Hover highlight on all interactive objects — items and register grow slightly under the cursor, signaling clickability
- Price tags on the shelf so the user knows costs before buying
- Shopkeeper gives spoken feedback for edge cases: counter full, checkout with an empty counter
- Complete shopping loop: after checkout she thanks the customer and the counter resets, so the demo can be replayed endlessly without restarting
- On-screen control hints at the bottom of the screen
- Polished look: warm lighting with soft shadows, ACES tonemapping, bloom and vignette via URP post-processing

## Architecture Overview

```
ClickManager  ──raycast──▶  IClickable (interface)
                              ├── ShelfItem ──▶ ShopManager (cart, spawning, fly effect, audio)
                              ├── Shopkeeper (wave animation, Speak)
                              └── CashRegister ──▶ ShopManager.Checkout()
                                                      └─▶ Shopkeeper.Speak ──▶ SpeechBubble (world-space, billboarded)
```

- A single `ClickManager` raycasts mouse clicks and hover; anything interactive implements `IClickable` — adding new interactive objects requires no changes to input code.
- Item definitions (`name`, `price`) are **ScriptableObjects** (`ItemData`), so the shop's inventory is data-driven and editable without touching code.
- `ShopManager` is the single source of truth for the cart, counter slots, and purchase flow.

## AI Tool Disclosure

As invited in the task instructions, I used **Claude (Anthropic)** as a development assistant throughout this project, in a pair-programming style:

- **Code**: drafting the C# scripts (click system, shop logic, speech bubble, effects), which I then integrated, wired up in the editor, tested, and iterated on
- **Guidance**: Debugging support based on screenshots of issues I hit

All Unity editor work — scene construction, component wiring, animation setup, materials, lighting, UI layout, testing, and all UX decisions — was done by me, and the step-by-step commit history documents the actual development process. I see directing AI tools effectively while owning and understanding the result as part of my engineering workflow.

## Future Extension Ideas (AI × VR)

Ways I would extend this demo toward PsyCurio's domain, given more time:

1. **LLM-driven shopkeeper dialogue** — replace canned lines with a small local or API-based language model so the shopkeeper converses naturally, remembers the customer's purchases, and adapts her personality; with graceful fallback to scripted lines when offline.
2. **Voice interaction** — speech-to-text input so users can talk to the shopkeeper hands-free, which maps directly to VR where keyboards are unavailable.
3. **Behavioral interaction analytics** — log gaze/click/hesitation patterns during shopping tasks; in a VR-psychology context, such interaction traces can support assessment and training scenarios.
4. **Computer-vision-driven embodiment** — using pose estimation to mirror the user's gestures onto an avatar for richer social presence.

## Credits

- Character and animations: [Mixamo](https://www.mixamo.com) (Adobe)
- Sound effects: [freesound.org](https://freesound.org) — *(CC0; authors: ADD_AUTHOR_NAMES_HERE)*
- Built with Unity 6.3 LTS & Universal Render Pipeline

---

**Aryan Kumar Singh (AK)** · [GitHub](https://github.com/aksingh1608) · [Portfolio](https://bewithaksingh.com)
