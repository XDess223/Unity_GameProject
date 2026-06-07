# Vertirious: 2D Adventure Framework

An advanced, feature-rich 2D adventure platformer built with **Unity 6 (6000.0.32f1)** and **C#**. This project was developed as a CSC490 Final Project, showcasing standard game architecture patterns, physics-based controls, custom enemy AI, level state management, and polish (such as parallax layers, UI systems, and dynamic sound libraries).

---

## Game Overview & Mechanics

**Vertirious** features a responsive player controller, interactive levels, and smart enemies. Players traverse platforms, solve navigation challenges, collect essences, manage power-up buffs, and combat intelligent enemies.

### Core Features

*   **Responsive Player Controls:** Physics-based (Rigidbody2D) movement supporting walking, running, jumping, attacking, and wall collision detection.
*   **Combat & Damage System:** A modular health/damage system (`ScriptDamageable`) for both the player and enemies. Features melee attack hitboxes, customizable knockback, damage cooldowns, and a lives management system.
*   **Intelligent Enemy AI:** Enemies with patrol states, cliff edge detection, line-of-sight tracking, and aggro zones. They dynamically seek and pursue the player, and drop collectibles upon defeat.
*   **Interactive Level Elements:** 
    *   **Moving Platforms:** Smoothly carry the player across hazards.
    *   **Moveable Obstacles:** Objects that the player can push or pull.
    *   **Destroyable Obstacles:** Barriers that can be broken by player attacks.
*   **Persistent Game State:** A global `GameManager` that persists player health, lives, and essence counts across scene transitions (moving forward/backward between doors).
*   **Visuals & Polish:** 
    *   Dynamic multi-layered Parallax scrolling effect.
    *   A custom UI HUD displaying player status, collected essences, and active power-ups (with countdown timers).
*   **Audio System:** Managed background music (`MusicManager`) and a modular sound effects player (`SoundEffectManager`) with an effects library.

---

## Tech Stack & Unity Components

*   **Game Engine:** Unity 6 (6000.0.32f1)
*   **Render Pipeline:** Universal Render Pipeline (URP)
*   **Input System:** Unity's New Input System (`inputactions` package)
*   **Physics:** Rigidbody2D, BoxCollider2D, PhysicsMaterial2D for friction management
*   **Scripting Language:** C#

---

## Key Architecture & Scripts

The codebase is structured cleanly inside the `Assets/Scripts/` folder:

*   **`PlayerManager.cs`**: Coordinates player inputs, movement speeds (airborne, running, walking), attack/jump triggers, state flags, and UI updates.
*   **`GameManager.cs`**: Handles global game states, level load transitions, save points, player health caching, and win/loss conditions.
*   **`ScriptDamageable.cs`**: A reusable health component shared by the player and enemies. Manages hit animations, invincibility frames, and event triggers (like death or particle bursts).
*   **`EnemyAgroScript.cs` & `EnemyPatrolScript.cs`**: Controls enemy behaviors, transitions between passive patrolling and aggressive hunting upon player detection.
*   **`PlayerMoveItemHandler.cs`**: Manages the physics and interactions for pushable/pullable game objects.

---

## Controls

*   **Move Left / Right:** `A` / `D` or Left / Right Arrows
*   **Jump:** `Space`
*   **Run:** Hold `Left Shift`
*   **Attack:** `Left Mouse Click` or `J`

---

## How to Setup and Run Locally

### Prerequisites
*   **Unity Hub**
*   **Unity Editor version `6000.0.32f1`** (or a compatible Unity 6 release)

### Steps to Run
1.  **Clone the Repository:**
    ```bash
    git clone https://github.com/YOUR_GITHUB_USERNAME/Unity_GameProject.git
    ```
2.  **Open in Unity:**
    *   Open Unity Hub.
    *   Click **Add** -> **Add project from disk**.
    *   Select the directory containing the cloned project.
    *   Open the project (Unity will restore the `Library/` folder automatically on first load).
3.  **Play the Game:**
    *   Navigate to the `Assets/Scenes/` folder.
    *   Open the main menu scene (or Level 1).
    *   Press the **Play** button in the Unity Editor.
