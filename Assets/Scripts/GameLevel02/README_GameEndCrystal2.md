# GameEndCrystal2 System for GameLevel02

This system implements a cinematic game ending sequence for GameLevel02 that triggers when all players reach the crystal and loads "Level03-1" instead of "GameLevel02".

## What's New

1. **New Scripts**: `GameEndCrystal2.cs` and `GameEndManager2.cs` specifically for GameLevel02
2. **Different Scene**: Loads "Level03-1" instead of "GameLevel02"
3. **Same Functionality**: All other features remain identical to the original system

## Files Created

- `GameEndCrystal2.cs` - Crystal script that uses GameEndManager2
- `GameEndManager2.cs` - Manager script that loads "Level03-1"
- `GameLevel02CrystalSetup.cs` - Setup utility script for easy configuration

## Key Differences from Original

- **Script Names**: `GameEndCrystal2` instead of `GameEndCrystal`
- **Manager**: `GameEndManager2` instead of `GameEndManager`
- **Next Scene**: "Level03-1" instead of "GameLevel02"
- **Everything Else**: Identical functionality and features

## Setup Instructions

### Method 1: Using the GameLevel02CrystalSetup Script (Recommended)

1. Create an empty GameObject in your GameLevel02 scene
2. Add the `GameLevel02CrystalSetup` script to it
3. Configure the settings in the inspector:
   - **Crystal Position**: Where to place the crystal (default: 44, -9, 0)
   - **End Camera Position**: Where the camera should move to (default: 22, -5)
   - **End Camera Zoom**: How much to zoom out (default: 12)
   - **Speed Settings**: Adjust timing for various effects
4. Right-click on the GameLevel02CrystalSetup component and select "Setup Complete GameLevel02 End"

This will automatically:
- Remove any old LevelExit components
- Create the new GameLevel02EndCrystal with GameEndCrystal2 script
- Position it correctly in the scene
- Configure all settings automatically

### Method 2: Manual Setup

1. Create a GameObject for the crystal
2. Add the `GameEndCrystal2` script to it
3. Configure all the settings in the inspector
4. Ensure there's a `GameEndManager2` in the scene (the script will create one if needed)

### Method 3: Update Existing Prefab

1. Drag the `GameEndCrystal2.prefab` from Resources into your scene
2. Add the `GameLevel02CrystalSetup` script to any GameObject
3. Right-click on the GameLevel02CrystalSetup component and select "Setup Complete GameLevel02 End"
4. This will create a new crystal with the correct GameEndCrystal2 script

## Configuration Options

### Game End Settings
- **End Camera Position**: X,Y coordinates where the camera should move
- **End Camera Zoom**: Orthographic size for the zoomed out view
- **Camera Move Speed**: How fast the camera moves (units per second)
- **Camera Zoom Speed**: How fast the camera zooms (units per second)
- **Vision Reveal Speed**: How fast global lighting increases to reveal full vision (seconds)
- **White Fade Speed**: How fast the white fade occurs (seconds)
- **Scene Transition Delay**: Delay before loading Level03-1 (seconds)

### Crystal Settings
- **Is Trigger**: Whether the collider should be a trigger
- **Trigger Size**: Size of the trigger collider

## How It Works

1. **Detection**: The system detects when players collide with the crystal
2. **Player Counting**: It tracks how many players have touched the crystal
3. **Trigger**: When all players (or single player in dev mode) touch the crystal, the sequence starts
4. **Sequence**: Uses GameEndManager2:
   - Global lighting gradually increases to reveal full vision
   - Camera moves to end position and zooms out
   - White fade covers the screen
   - Scene transitions to "Level03-1"
   - All features are reset before transition

## Scene Transition

The system will load "Level03-1" when the crystal is triggered. Make sure:
- The "Level03-1" scene is added to Build Settings
- The scene name is exactly "Level03-1" (case-sensitive)

## Photon PUN 2 Integration

- Works with both single-player (developer mode) and multiplayer
- Automatically detects player count from Photon room
- Master client handles scene loading in multiplayer
- All clients see the same ending sequence

## Troubleshooting

- **Script not found**: Make sure all scripts are compiled and in the correct folders
- **Scene not loading**: Ensure "Level03-1" scene is in Build Settings
- **No global lights found**: The system will automatically create a global light if none exists
- **Camera not moving**: Check that Main Camera exists and is tagged correctly

## Notes

- The system automatically creates a GameEndManager2 if one doesn't exist
- All timing can be adjusted in the inspector
- The system works with both Photon PUN 2 and single-player modes
- This is a separate system from GameLevel01, so changes don't affect the original
