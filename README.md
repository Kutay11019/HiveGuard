# 🐝 HiveGuard: Honey Empire

HiveGuard: Honey Empire is a mobile action-survival game where the player controls a worker bee, collects pollen during the day, and defends the hive against enemies at night.

The game is built around a fast day-night cycle: calm resource collection during the day turns into tense hive defense at night.

---

## 🎮 Overview

In **HiveGuard: Honey Empire**, the player must help a bee colony survive by collecting pollen, delivering it to the hive, and protecting the hive from nighttime attacks.

During the **day phase**, the player explores the meadow, collects pollen from flowers, and carries it back to the hive.

During the **night phase**, enemies attack the hive. The player must defend it by fighting against bears and wasps before the hive or the bee is defeated.

The current final version focuses on a complete playable mobile experience with:

- Day and night gameplay phases
- Pollen collection and delivery
- Hive defense
- Enemy waves
- Player and hive health systems
- Mobile joystick and attack controls
- Win and lose conditions
- Main menu and How to Play screens

---

## 🔄 Core Gameplay Loop

The game follows a repeated day-night survival loop:

1. 🌼 **Day Phase**
   - Explore the meadow
   - Collect pollen from flowers
   - Deliver pollen to the hive
   - Prepare for the upcoming night

2. 🌙 **Night Phase**
   - Pollen collection stops
   - Enemy waves begin
   - Bears and wasps attack the hive and/or the player
   - The player must protect the hive until the night ends or all enemies are defeated

3. 🔁 **Progression**
   - If the player survives the night, the next day begins
   - Enemy difficulty increases as the days progress
   - The player must survive until the final day

---

## 🎯 Player Objective

The main objective is to keep the hive alive until the end of the game.

The player must:

- Collect pollen during the day
- Deliver pollen to the hive
- Defend the hive during the night
- Attack and defeat incoming enemies
- Survive all days without the bee or hive being destroyed

---

## 🏆 Win Condition

The player wins by surviving all planned days and successfully protecting the hive through the final night.

Current final version win condition:

- Reach **Day 5**
- Survive the final night
- Defeat or outlast the Bears and Wasps

---

## ❌ Failure Conditions

The player loses if:

- 🐝 The bee's health reaches zero
- 🏚️ The hive's health reaches zero

When the player loses, the game shows a defeat screen and allows the player to restart or return to the main menu.

---

## 🕹️ Controls

The game is designed for mobile gameplay.

### Mobile Controls

- **Joystick**: Move the bee
- **Attack Button**: Attack nearby enemies
- **Hive Delivery**: Move to the hive while carrying pollen to deliver it

The UI is designed to be readable and usable on mobile screens.

---

## 🧠 Gameplay Systems

### 🌼 Pollen Collection System

During the day, pollen appears around the meadow. The player collects pollen by moving the bee into pollen objects.

Collected pollen is carried by the bee and can be delivered to the hive.

### 🏠 Hive Delivery System

The player must return to the hive to deliver collected pollen.

Pollen delivery is only active during the day phase.

### 🌙 Day-Night Cycle

The game automatically switches between day and night phases.

- Day phase focuses on collection
- Night phase focuses on defense
- Lighting and atmosphere change between phases

### 🐻 Enemy System

Enemies spawn during the night and threaten the hive.

Current enemy types include:

- **Bear**
  - Strong enemy
  - Attacks the hive
  - Has more health

- **Wasp**
  - Faster enemy
  - Can threaten the bee and/or hive
  - Has lower health than bears

### ❤️ Health System

Both the bee and the hive have health.

- If the bee dies, the player loses
- If the hive is destroyed, the player loses

### 📈 Difficulty Progression

Each new night becomes more difficult by increasing enemy pressure.

The game gradually introduces more threats as the player progresses through the days.

---

## 🧩 Features

- Mobile-friendly gameplay
- Main menu screen
- How to Play screen
- Day-night cycle system
- Pollen collection mechanic
- Hive delivery mechanic
- Bear enemy AI
- Wasp enemy AI
- Enemy wave spawning
- Bee health system
- Hive health system
- Attack system
- Victory and defeat screens
- Restart and main menu options
- Improved mobile UI
- Background music support
- Android build support

---

## 🌍 World & Theme

HiveGuard takes place in a colorful forest meadow centered around a bee hive.

The visual style is designed to be:

- Bright and friendly during the day
- Darker and more tense during the night
- Cartoon-like and accessible
- Suitable for a casual mobile game

---

## 🎨 Design Pillars

### 🌼 Calm to Chaos

The game contrasts peaceful daytime collection with dangerous nighttime defense.

### 🐝 Simple but Active Gameplay

The player always has a clear task:

- Collect during the day
- Defend during the night

### ⏱️ Short Session Design

The game is designed around short gameplay cycles, making it suitable for quick mobile sessions.

### 🎯 Clear Objectives

The UI guides the player by showing the current phase, timer, health information, and objectives.

---

## 🛠️ Tech Stack

- **Engine:** Unity
- **Platform:** Android / Mobile
- **Programming Language:** C#
- **Version Control:** GitHub
- **Project Management:** Jira
- **Audio Editing:** Audacity
- **3D / Visual Assets:** Unity Asset Store and custom adjustments

---

## 📱 Platform

The final build is prepared for Android devices.

The game uses mobile input controls such as an on-screen joystick and attack button.

---

## 📌 Project Status

Final playable version completed.

The submitted version includes the core gameplay loop, mobile controls, enemy waves, UI screens, and win/lose conditions.

---

## 🤝 Contributors

- Kutay Ayoğlu
- Sena Akbaba
