# 🎵 RMP

**RMP** is a C# project for managing music metadata, playing music, and browsing/searching files.  
It features a console-based UI, logging, state management, and theme support. Modular and easy to extend, RMP works well for small music apps or as a framework for larger projects.

---

## ✨ Features

- 📂 **Scan directories** to find music files  
- 📝 **Store metadata** for songs  
- ▶️ **Play songs** via a simple playback system  
- 🖥️ **Console-based UI** for easy interaction  
- 🎨 **Theme management** across the application  
- 🛠️ **Logging** of errors and info to `logs.txt`  
- 💾 **State persistence** for settings  

---

## 📁 Project Structure

```
RMP/
├── RMP.slnx
├── .editorconfig
├── CLAUDE.md
├── .claude
├── RMP/
│   ├── Interfaces/       # 🔧 Interface definitions for services and modules
│   ├── Services/         # ⚙️ Services like LogService, StateService, etc.
│   ├── FileBrowser.cs    # 📂 Handles scanning directories for music files
│   ├── MusicPlayback.cs  # ▶️ Handles music playback functionality
│   ├── Program.cs        # 🚀 Entry point of the application
│   ├── Settings.cs       # ⚙️ Manages application settings
│   ├── SimpleUI.cs       # 🖥️ Console-based UI implementation
│   ├── StateService.cs   # 💾 Saves and loads state across sessions
│   ├── ThemeChanger.cs   # 🎨 Handles themes across UI and components
│   ├── Linq.cs           # 🔍 LINQ-based searches for songs/artists
│   └── RMP.csproj        # 📦 Project file
```

---

## 📖 Class Descriptions

### 🚀 Program.cs  
Main entry point of the console application. Starts the app.

### ▶️ MusicPlayback.cs  
Plays songs from your local music folder in random order. Skip tracks or select a song using a beautiful Spectre Console UI.

### 📂 FileBrowser.cs  
Scans your local music folder and prompts you to select a song. If no songs are found, it shows:  
`"No MP3 files located"`

### 🔍 Linq.cs  
Search for songs or artists with a query. Returns all matching files quickly using LINQ.

### 🖥️ SimpleUI.cs  
Main menu of the app. Instantiates all other classes to execute their functionality.

### ⚙️ Settings.cs  
Manage app settings like theme and volume. Settings are saved to a JSON file and loaded automatically at startup (`RMP\bin\Debug\net8.0\settings.cs`).

### 💾 StateService.cs  
Provides generic methods to save and load **any object**. No need to write separate JSON methods for different types—flexibility at its best!

### 🎨 ThemeManager.cs  
Change the console theme. Available colors: `⚫"Dark"`, `🟣"Purple"`, `🔵"Blue"`, `🟢"Green"`, `🟡"Yellow"`, `🔴"Red"` (more coming soon). Selected theme is saved to `Settings.json`.

### 🔧 ILogService.cs  
Interface for logging **info, warnings, and errors** throughout the app.

> 💡 **Interface = Contract**  
> An interface defines a **contract** that any implementing class must fulfill.  
> - Specifies **what methods, properties, events, or indexers** a class must implement  
> - Does **not provide implementation**  
> - Think of it as a **promise**: the class guarantees the behavior defined by the interface.

---
