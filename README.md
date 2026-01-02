# Morph Runner 🎮  
**A Data-Driven 2D Runner Game Built with Unity & Firebase**

Morph Runner is a **level-based 2D runner** developed in **Unity (C#)** that challenges players to survive by **dynamically morphing shapes and colors** in real time. The project emphasizes **game systems design, physics, analytics-driven iteration, and scalable architecture**, making it a strong demonstration of real-world software engineering principles applied to interactive systems.

---

## 🚀 Why This Project Matters

This project demonstrates:
- **Complex state management** (shape, color, stacking, health)
- **Physics & collision system design**
- **User behavior analytics & telemetry**
- **Iterative development driven by real player data**
- **Clear ownership across gameplay systems**
- **End-to-end delivery** from concept → alpha → beta → gold

Morph Runner was built, tested, analyzed, and refined through multiple playtest cycles using **Firebase-backed analytics**, mirroring real production workflows used in industry game and software teams.

---

## 🧠 Core Gameplay Systems

### 1️⃣ Shape Morphing System
- Players collect shape pickups to morph into:
  - Circle
  - Triangle
  - Square
- Correct shape matching:
  - Allows obstacle traversal
  - Restores player health
- Incorrect matching:
  - Triggers health penalties
  - Activates visual & physical feedback

**Engineering focus:**  
State transitions, collision validation, fail-safe handling, and deterministic obstacle checks.

---

### 2️⃣ Color Matching System (Levels 2 & 4)
- Introduces an orthogonal state (color) layered on top of shape logic
- Requires players to match **both shape and color** simultaneously

**Engineering focus:**  
Multi-constraint validation, input handling, layered game-state evaluation.

---

### 3️⃣ Shape Stacking System (Levels 3 & 4)
- Shapes can stack to form compound morph states
- Obstacles require matching stacked layouts

**Engineering focus:**  
Data structures for shape stacking, forward-planning mechanics, and compound collision logic.

---

### 4️⃣ Health & Risk–Reward System
- Health drains passively over time
- Correct matches restore health
- Incorrect matches accelerate failure

**Engineering focus:**  
Dynamic tuning, time-based decay, difficulty balancing across levels.

---

## 🕹️ Controls

| Action | Input |
|------|------|
| Move Left | A / ← |
| Move Right | D / → |
| Change Color | Space |
| Pause | In-game UI |

---

## 🗺️ Level Progression & Difficulty Scaling

| Level | New Systems Introduced | Goal |
|------|-----------------------|------|
| Level 1 | Shape matching | Teach core mechanic |
| Level 2 | Shape + color | Introduce dual constraints |
| Level 3 | Shape stacking | Enforce planning |
| Level 4 | Stack + color | Full system mastery |

Each level **depends on mastery of prior mechanics**, preventing shallow “trial-and-error” gameplay.

---

## 📊 Analytics & Telemetry (Firebase)

Morph Runner integrates **Firebase Realtime Database** to collect live gameplay data.

### Metrics Tracked
- Obstacle match accuracy
- Death rate per level
- Remaining health at completion
- Y-position of failed obstacle collisions
- Correct vs incorrect match ratio

### Why This Matters
- Enabled **data-backed difficulty tuning**
- Identified tutorial failures and difficulty spikes
- Improved retention by adjusting punishment curves

Analytics data was exported as JSON and visualized using **Python (matplotlib)**.

---

## 🔁 Iterative Improvements Based on Data

### Problems Identified
- Excessive early deaths
- Poor visibility of upcoming obstacles
- Collider mismatch causing unfair deaths
- Health decay too aggressive
- No pause functionality

### Solutions Implemented
- Added pause system
- Tuned collider dimensions to match visuals
- Expanded camera field of view
- Reduced passive health decay interval
- Improved UI feedback and tutorials

This cycle reflects **real-world production debugging and iteration**.

---

## 👨‍💻 My Contributions — Akhil Joseph

**Role:** Physics & Level Design Engineer

Key contributions:
- Designed and implemented **shape-based obstacle physics**
- Built Level 1 & Level 2 gameplay logic
- Implemented **collision tuning & fail-state recovery**
- Added player slowdown & camera shake feedback
- Developed tutorialization for multiple levels
- Implemented pause system
- Analyzed playtest data and shipped balance fixes

This work required **cross-functional collaboration**, debugging player-reported issues, and translating analytics into concrete engineering changes.

---

## 🛠️ Tech Stack

- **Unity (C#)**
- **Firebase Realtime Database**
- **WebGL**
- **Python (Analytics & Visualization)**

---

## 🔮 Future Enhancements

- Procedural level generation
- Leaderboards
- Combo-based reward systems
- Adaptive difficulty scaling
- Expanded morph mechanics
