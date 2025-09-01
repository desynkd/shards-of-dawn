# Multiplayer Door System - GameLevel01

## Overview
This system implements a multiplayer pressure button puzzle in GameLevel01 that requires players to press buttons in specific sequences based on the number of players in the room.

## Features

### Multiplayer Support
- **2 Players**: Sequence 01 → 04 (must press in order)
- **3 Players**: Sequence 01 → 04 → 05 (must press in order)  
- **4 Players**: All 3 buttons must be pressed simultaneously (order doesn't matter)

### Sequence Validation
- If buttons are pressed in the wrong order, the scene automatically restarts
- All button interactions are synchronized across all clients using Photon RPC calls
- Only the Master Client can restart the scene to prevent conflicts

### Button Behavior
- **2-3 Players**: Buttons stay pressed once activated
- **4 Players**: Buttons are pressure-sensitive (door reactivates when all players step off)

## Setup Instructions

### 1. GameObject Setup
1. Create a GameObject in your scene (e.g., "MultiplayerDoor01Trigger")
2. Add the `MultiplayerDoor01Trigger` component
3. Add the `MultiplayerDoorSetup` component (optional - auto-configures PhotonView)

### 2. Assign References
In the `MultiplayerDoor01Trigger` component, assign:
- **Door To Deactivate**: The door GameObject that should be disabled when puzzle is solved
- **Pressure Button 01**: The first button GameObject
- **Pressure Button 04**: The second button GameObject  
- **Pressure Button 05**: The third button GameObject
- **Door Reactivate Delay**: Time in seconds before door reactivates (4-player mode)

### 3. Button Scripts
Each pressure button should have the appropriate script:

#### PressureButton01
- Assign `targetToDeactivate` for single-player mode
- Assign `doorTrigger` reference to the MultiplayerDoor01Trigger GameObject
- Assign `pressedSprite` for visual feedback

#### PressureButton04 & PressureButton05
- Assign `doorTrigger` reference to the MultiplayerDoor01Trigger GameObject
- Assign `pressedSprite` for visual feedback

### 4. Photon Networking
The system automatically handles:
- RPC calls for button press synchronization
- Scene restart coordination (Master Client only)
- Player count detection and button activation

## Scripts Overview

### MultiplayerDoor01Trigger.cs
Main controller script that:
- Manages button sequences based on player count
- Handles RPC calls for multiplayer synchronization
- Controls door activation/deactivation
- Manages scene restarting

### PressureButton01.cs
Button script that:
- Integrates with the door trigger system
- Supports both single-player and multiplayer modes
- Provides visual feedback when pressed

### MultiplayerDoorSetup.cs
Optional setup script that:
- Automatically adds PhotonView component
- Configures PhotonView settings
- Ensures proper component relationships

## Sequence Rules

### 2 Players
1. Player 1 must press Button 01 first
2. Player 2 must press Button 04 second
3. If Button 04 is pressed before Button 01, scene restarts

### 3 Players  
1. Player 1 must press Button 01 first
2. Player 2 must press Button 04 second
3. Player 3 must press Button 05 third
4. If any button is pressed out of order, scene restarts

### 4 Players
1. All 3 buttons must be pressed simultaneously
2. Order doesn't matter
3. Door deactivates when all buttons are pressed
4. Door reactivates after delay when all players step off buttons

## Debug Information
The system provides debug logs for:
- Button press events
- Sequence validation
- Scene restart triggers
- Multiplayer synchronization

## Troubleshooting

### Common Issues
1. **Buttons not responding**: Check that `doorTrigger` reference is assigned
2. **Multiplayer not working**: Ensure PhotonView component is present
3. **Scene not restarting**: Verify Master Client status and Photon networking

### Debug Steps
1. Check console for debug messages
2. Verify player count detection
3. Ensure all button references are assigned
4. Test in both single-player and multiplayer modes

## Network Requirements
- Requires Photon Unity Networking (PUN) 2
- All clients must be in the same Photon room
- Master Client controls scene restarting
- RPC calls ensure synchronization across all clients
