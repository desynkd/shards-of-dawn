# Shards of Dawn

![Unity](https://img.shields.io/badge/Unity-2022.3+-blue.svg)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey.svg)
![Multiplayer](https://img.shields.io/badge/Multiplayer-Photon%20PUN2-green.svg)

## About

**Shards of Dawn** is a 2D multiplayer platformer game developed as a group project for a Game Development module at university. The game tells the story of four chosen bearers of light who must work together to restore the shattered realm.

## Story

When the Brilliant Shard of Light was destroyed, the realm collapsed into eternal night. Four chosen bearers of light — each holding a fragment of the lost shard — must journey across the broken world. Their task: to reawaken each shard by venturing into the darkness, overcoming trials, and combining their flames at the core of each fragment.

## Features

### 🎮 Core Gameplay
- **2D Platformer Mechanics**: Classic side-scrolling platformer with precise movement and jumping
- **Multiplayer Co-op**: Support for 1-4 players working together
- **Puzzle Elements**: Pressure button puzzles that require coordination between players
- **Dynamic Platforms**: Falling platforms and interactive environmental elements
- **Multiple Levels**: Various themed levels with unique challenges

### 🌐 Multiplayer Features
- **Photon PUN2 Integration**: Real-time multiplayer networking
- **Player Synchronization**: All player actions and interactions are synchronized across clients
- **Adaptive Puzzles**: Button sequences and puzzles that scale based on the number of players
- **Lobby System**: Join/create rooms and wait for other players
- **Cross-Platform**: Works across different devices and platforms

### 🎯 Game Levels
- **Cutscene01 & Cutscene02**: Story introduction scenes
- **GameLevel01**: First main level with complex multiplayer button puzzles
- **GameLevel02**: Second level with simplified pressure button mechanics
- **Level03-1, Level03-2, Level03-3**: Additional challenge levels
- **Lobby & WaitingRoom**: Multiplayer matchmaking areas

### 🎨 Visual & Audio
- **Pixel Art Style**: Beautiful 2D pixel art assets
- **Animated Sprites**: Smooth character and environment animations
- **Background Music**: Immersive audio experience
- **Visual Effects**: Particle effects and dynamic lighting

## Technical Details

### Built With
- **Unity 2022.3+**: Game engine and development platform
- **Photon PUN2**: Multiplayer networking solution
- **Unity Input System**: Modern input handling
- **C#**: Programming language
- **2D Physics**: Unity's 2D physics system for realistic movement

### Key Systems
- **Player Controller**: Responsive movement with ground detection and jump mechanics
- **Multiplayer Door System**: Complex puzzle mechanics that adapt to player count
- **Falling Platforms**: Dynamic environmental hazards
- **Enemy AI**: Basic patrol and interaction systems
- **Scene Management**: Seamless transitions between levels

## Installation & Setup

### Prerequisites
- Unity 2022.3 or later
- Windows 10/11 (primary platform)
- Internet connection for multiplayer features

### Running the Game
1. Clone this repository
2. Open the project in Unity 2022.3+
3. Open the `Lobby` scene
4. Press Play to start the game
5. Create or join a room to play with others

### Building the Game
1. Go to File → Build Settings
2. Add the main scenes to the build
3. Select Windows platform
4. Click Build to create an executable

## Gameplay Instructions

### Controls
- **Movement**: A/D or Arrow Keys
- **Jump**: Spacebar or W
- **Variable Jump**: Hold Spacebar for higher jumps, release for shorter jumps

### Multiplayer Modes
- **1 Player**: Single-player experience with simplified puzzles
- **2 Players**: Button sequence puzzles requiring coordination
- **3 Players**: Extended sequences with three-button combinations
- **4 Players**: Simultaneous button pressing challenges

### Puzzle Mechanics
- **Pressure Buttons**: Step on buttons to activate mechanisms
- **Sequential Puzzles**: Press buttons in specific orders (2-3 players)
- **Simultaneous Puzzles**: All players must press buttons together (4 players)
- **Door Systems**: Buttons control access to new areas

## Development Team

This project was developed as part of a university Game Development module by a collaborative team of students. The game showcases various aspects of game development including:

- Game design and mechanics
- Multiplayer networking
- Level design and puzzle creation
- Asset integration and optimization
- User interface design

## Project Structure

```
Assets/
├── Scripts/           # Game logic and systems
├── Scenes/           # Game levels and cutscenes
├── GDD - Quinnipiac/ # Game design documents and assets
├── Photon/           # Multiplayer networking
├── Platformer Pack/  # Art assets and sprites
├── Music/            # Audio files
└── Settings/         # Game configuration
```

## Contributing

This is an academic project developed for educational purposes. While contributions are welcome, please note that this project was created as part of a university course.

## License

This project is created for educational purposes as part of a university Game Development module. Please respect the academic nature of this work.

## Acknowledgments

- Unity Technologies for the game engine
- Photon for multiplayer networking solutions
- Various asset creators whose work contributed to the game's visual and audio experience
- The university Game Development module instructors and fellow students

---

*"In the darkness, light finds a way. Together, we shall restore what was lost."*
