# Game End System for GameLevel02

This system implements the same cinematic game ending sequence from GameLevel01 for GameLevel02, making the code reusable and configurable.

## What Changed

1. **Removed**: Old `LevelExit` component that provided simple level completion
2. **Added**: New `GameLevel02EndCrystal` that reuses the `GameEndManager` logic
3. **Made Configurable**: The next scene is now configurable instead of hardcoded

## Features

1. **Full Vision Reveal**: Global lighting gradually increases to give players full vision of the entire scene
2. **Camera Movement**: Camera moves to a defined position and zooms out to show the full level
3. **White Fade**: The level gradually becomes white before transitioning to the next scene
4. **Feature Reset**: All modified features are reset before entering the next scene
5. **Configurable Next Scene**: The next scene can be set in the inspector

## Setup Instructions

### Method 1: Using the GameLevel02CrystalSetup Script (Recommended)

1. Create an empty GameObject in your GameLevel02 scene
2. Add the `GameLevel02CrystalSetup` script to it
3. Configure the settings in the inspector:
   - **Crystal Position**: Where to place the crystal (default: 44, -9, 0)
   - **End Camera Position**: Where the camera should move to (default: 22, -5)
   - **End Camera Zoom**: How much to zoom out (default: 12)
   - **Speed Settings**: Adjust timing for various effects
   - **Next Scene Name**: What scene to load next (default: "Level03-1")
4. Right-click on the GameLevel02CrystalSetup component and select "Setup Complete GameLevel02 End"

This will automatically:
- Remove the old LevelExit component
- Create the new GameLevel02EndCrystal
- Position it correctly in the scene

### Method 2: Manual Setup

1. Create a GameObject for the crystal
2. Add the `GameLevel02EndCrystal` script
3. Configure all the settings in the inspector
4. Ensure there's a `GameEndManager` in the scene (the script will create one if needed)

## Configuration Options

### Game End Settings
- **End Camera Position**: X,Y coordinates where the camera should move
- **End Camera Zoom**: Orthographic size for the zoomed out view
- **Camera Move Speed**: How fast the camera moves (units per second)
- **Camera Zoom Speed**: How fast the camera zooms (units per second)
- **Vision Reveal Speed**: How fast global lighting increases to reveal full vision (seconds)
- **White Fade Speed**: How fast the white fade occurs (seconds)
- **Scene Transition Delay**: Delay before loading the next scene (seconds)
- **Next Scene Name**: The name of the scene to load next

### Crystal Settings
- **Is Trigger**: Whether the collider should be a trigger
- **Trigger Size**: Size of the trigger collider

## How It Works

1. **Detection**: The system detects when players collide with the crystal
2. **Player Counting**: It tracks how many players have touched the crystal
3. **Trigger**: When all players (or single player in dev mode) touch the crystal, the sequence starts
4. **Sequence**: Uses the same `GameEndManager` as GameLevel01:
   - Global lighting gradually increases to reveal full vision
   - Camera moves to end position and zooms out
   - White fade covers the screen
   - Scene transitions to the configured next scene
   - All features are reset before transition

## Code Reusability

The system reuses the `GameEndManager` from GameLevel01, making it:
- **Maintainable**: Changes to the ending sequence only need to be made in one place
- **Consistent**: Both levels will have identical ending experiences
- **Configurable**: Each level can specify its own settings and next scene

## Photon PUN 2 Integration

- Works with both single-player (developer mode) and multiplayer
- Automatically detects player count from Photon room
- Master client handles scene loading in multiplayer
- All clients see the same ending sequence

## Troubleshooting

- **No global lights found**: The system will automatically create a global light if none exists
- **Camera not moving**: Check that Main Camera exists and is tagged correctly
- **Scene not loading**: Ensure the next scene name is correct and the scene is in Build Settings
- **Script not found**: Make sure all scripts are compiled and in the correct folders

## Files Created

- `GameLevel02EndCrystal.cs` - Main crystal script for GameLevel02
- `GameLevel02CrystalSetup.cs` - Utility script for easy setup
- `README_GameLevel02GameEnd.md` - This documentation file

## Notes

- The system automatically creates a GameEndManager if one doesn't exist
- All timing can be adjusted in the inspector
- The system works with both Photon PUN 2 and single-player modes
- The old LevelExit component is automatically removed during setup
