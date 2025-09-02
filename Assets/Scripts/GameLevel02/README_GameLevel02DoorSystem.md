# GameLevel02 Door System

## Overview
This system implements a simple pressure button puzzle in GameLevel02 that deactivates a door tilemap when any player steps on the button. The system provides immediate local feedback and synchronizes the door state across all players in the Photon room.

## Features

### Single Player Functionality
- **Immediate Response**: Door deactivates instantly when the local player steps on the button
- **Visual Feedback**: Button sprite changes to pressed state
- **Permanent Effect**: Once activated, the door stays deactivated

### Multiplayer Synchronization
- **RPC Communication**: Uses Photon RPC calls to sync door state across all clients
- **Client Validation**: Only processes button presses from the local player
- **Network Consistency**: Ensures all players see the same door state

## Setup Instructions

### 1. GameObject Setup
1. Create a GameObject in your GameLevel02 scene (e.g., "MultiplayerDoor02Trigger")
2. Add the `MultiplayerDoor02Trigger` component
3. Add the `MultiplayerDoor02Setup` component (auto-configures PhotonView)

### 2. Assign References
In the `MultiplayerDoor02Trigger` component, assign:
- **Door To Deactivate**: The door tilemap GameObject that should be disabled when the button is pressed
- **Pressure Button 01**: The PressureButton01 GameObject

### 3. Button Setup
Ensure your pressure button has:
- **Collider2D**: Set as trigger for player detection
- **PressureButton01 Script**: Attached with proper references
- **SpriteRenderer**: For visual feedback

### 4. Inspector Configuration
In the `PressureButton01` component, assign:
- **Door To Deactivate**: Same door tilemap as in the trigger component
- **Door Trigger**: Reference to the MultiplayerDoor02Trigger GameObject
- **Pressed Sprite**: The sprite to show when button is pressed
- **Player Tag**: Tag used by player objects (default: "Player")

## How It Works

### Button Press Detection
1. Player collides with button trigger
2. System checks if the colliding player is the local player
3. Button state changes to pressed
4. Door deactivates locally for immediate feedback
5. RPC call synchronizes door state across all clients

### Multiplayer Synchronization
- **OnButtonPressed()**: Sends RPC to all clients to deactivate door
- **RPC_OnButtonPressed()**: Receives RPC and deactivates door on all clients
- **Door State**: Once deactivated, door remains deactivated for all players

### Client-Side Validation
- Only processes button presses from the local player
- Prevents duplicate processing from network events
- Maintains responsive local gameplay

## Code Structure

### MultiplayerDoor02Trigger
- Main controller for door state management
- Handles RPC communication
- Manages door activation/deactivation

### PressureButton01
- Detects player collisions
- Validates local player interaction
- Triggers door system synchronization

### MultiplayerDoor02Setup
- Automatically configures Photon networking
- Ensures proper PhotonView setup
- Validates component configuration

## Troubleshooting

### Common Issues
1. **Door not deactivating**: Check if doorToDeactivate reference is assigned
2. **No multiplayer sync**: Verify PhotonView component exists and is configured
3. **Button not responding**: Ensure player has correct tag and trigger collider is set

### Debug Logs
The system provides detailed debug logging:
- Button press detection
- Door state changes
- RPC communication
- Setup validation

## Differences from GameLevel01
- **Simplified Logic**: Single button instead of complex sequences
- **Immediate Effect**: No waiting for multiple button presses
- **Permanent State**: Door stays deactivated once button is pressed
- **Single Player Focus**: Designed for individual player interaction with multiplayer sync

