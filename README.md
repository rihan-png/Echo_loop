# Echo Loop 🔁

> *Use your past self to escape the loop.*

## What is Echo Loop?

Echo Loop is a 2D puzzle-platformer built in Unity where time resets every 10 seconds. Every time the loop resets, a **ghost clone** of your past self appears and replays your exact movements. You must cooperate with your past self to solve puzzles that are impossible to complete alone in a single loop.

## How to Play

| Control | Action |
|---|---|
| `A` / `D` or `←` / `→` | Move left / right |
| `Space` | Jump |
| `R` | Manually reset the loop |

## Core Mechanic

1. You have **10 seconds** per loop.
2. When the loop resets, a **ghost clone** of you spawns and replays your last loop perfectly.
3. Use your ghost clone to **hold pressure plates** while you run through doors.
4. Multiple loops = multiple clones working together!

## Puzzle Walkthrough

- **Puzzle 1:** Stand on the red pressure plate → it turns green and opens the orange door. Press `R` to loop. Now run to the door — your ghost holds it open!
- **Puzzle 2:** Repeat the mechanic with the second green door further in the level.
- **Win:** Reach the glowing yellow zone at the end!

## Tech Stack

- **Engine:** Unity 6 (6000.1.15f1)
- **Language:** C#
- **Input:** Unity New Input System
- **Physics:** Unity 2D Physics (Rigidbody2D, Trigger Colliders)
- **Audio:** Procedurally generated tones (no external audio files)
- **UI:** Unity uGUI (Legacy UI)

## Project Structure

```
Assets/EchoLoop/
├── Scripts/
│   ├── Core/          # CameraFollow, SoundManager
│   ├── Player/        # PlayerController (movement + jump)
│   ├── Loop/          # LoopManager (timer, reset, ghost spawning)
│   ├── Echo/          # EchoData, EchoRecorder, EchoPlayer
│   ├── Puzzle/        # PressurePlate, Door, WinZone
│   └── UI/            # UIManager, MainMenuController
├── Scenes/
│   ├── MainMenu.unity
│   └── Prototype.unity
└── Prefabs/
    └── EchoClone.prefab
```

## Running the Project

1. Open the project in **Unity 6 (6000.1.15f1)** or later.
2. Open `Assets/EchoLoop/Scenes/MainMenu.unity`.
3. Press **Play** in the Unity Editor.
4. Press **Space** or click **PLAY** to begin.

## Team

- Developer: [Your Name]
- Hackathon: [Hackathon Name]

## AI Usage Declaration

AI tools (Antigravity by Google DeepMind) were used during development to assist with:
- Boilerplate C# script generation
- Unity Editor automation scripts
- Debugging compiler errors
- Project architecture planning

All game design decisions, puzzle design, and testing were performed by the developer.
