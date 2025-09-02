# Game End System for GameLevel01

This system implements a cinematic game ending sequence for GameLevel01 that triggers when all players reach the crystal.

## Features

1. **Full Vision Reveal**: Global lighting gradually increases to give players full vision of the entire scene
2. **Camera Movement**: Camera moves to a defined position and zooms out to show the full level
3. **White Fade**: The level gradually becomes white before transitioning to GameLevel02
4. **Feature Reset**: All modified features are reset before entering GameLevel02

## Setup Instructions

### Method 1: Using the CrystalSetup Script

1. Create an empty GameObject in your scene
2. Add the `CrystalSetup` script to it
3. Configure the settings in the inspector:
   - **Crystal Position**: Where to place the crystal (default: 10, 0, 0)
   - **End Camera Position**: Where the camera should move to (default: 0, 0)
   - **End Camera Zoom**: How much to zoom out (default: 15)
   - **Speed Settings**: Adjust timing for various effects
4. Right-click on the CrystalSetup component and select "Create Game End Crystal"

### Method 2: Using the GameEndCrystal Prefab

1. Drag the `GameEndCrystal.prefab` from the Resources folder into your scene
2. Position it where you want players to trigger the game end
3. Configure the settings in the inspector

### Method 3: Adding to Existing Crystal

1. Find an existing crystal object in your scene
2. Add the `GameEndCrystal` script to it
3. Configure the settings in the inspector

## Configuration Options

### Game End Settings
- **End Camera Position**: X,Y coordinates where the camera should move
- **End Camera Zoom**: Orthographic size for the zoomed out view
- **Camera Move Speed**: How fast the camera moves (units per second)
- **Camera Zoom Speed**: How fast the camera zooms (units per second)
- **Vision Reveal Speed**: How fast global lighting increases to reveal full vision (seconds)
- **White Fade Speed**: How fast the white fade occurs (seconds)
- **Scene Transition Delay**: Delay before loading GameLevel02 (seconds)

### Crystal Settings
- **Is Trigger**: Whether the collider should be a trigger
- **Trigger Size**: Size of the trigger collider

## How It Works

1. **Detection**: The system detects when players collide with the crystal
2. **Player Counting**: It tracks how many players have touched the crystal
3. **Trigger**: When all players (or single player in dev mode) touch the crystal, the sequence starts
4. **Sequence**:
   - Global lighting gradually increases to reveal full vision
   - Camera moves to end position and zooms out
   - White fade covers the screen
   - Scene transitions to GameLevel02
   - All features are reset before transition

## Photon PUN 2 Integration

- Works with both single-player (developer mode) and multiplayer
- Automatically detects player count from Photon room
- Master client handles scene loading in multiplayer
- All clients see the same ending sequence

## Troubleshooting

- **No global lights found**: The system will automatically create a global light if none exists
- **Camera not moving**: Check that Main Camera exists and is tagged correctly
- **Scene not loading**: Ensure GameLevel02 scene is in Build Settings
- **Script not found**: Make sure all scripts are compiled and in the correct folders

## Files Created

- `Crystal.cs` - Basic crystal script (can be replaced with GameEndCrystal)
- `GameEndCrystal.cs` - Main crystal script with trigger functionality
- `GameEndManager.cs` - Manages the entire ending sequence
- `CrystalSetup.cs` - Utility script for easy setup
- `GameEndCrystal.prefab` - Prefab for easy placement
- `PlayerTorch.cs` - Modified to support lighting effects

## Notes

- The system automatically creates a GameEndManager if one doesn't exist
- All timing can be adjusted in the inspector
- The system works with both Photon PUN 2 and single-player modes
- Visual debugging is available (yellow wireframe shows trigger area when selected)
