# Photon Scene Synchronization Troubleshooting

## Issue: Scene Restart Not Synchronized Across All Clients

### Problem Description
When the Master Client restarts the scene due to wrong button sequence, other clients remain in the previous scene state instead of following the restart.

### Root Causes and Solutions

#### 1. **PhotonNetwork.AutomaticallySyncScene Setting**
**Problem**: The `AutomaticallySyncScene` setting might be disabled.
**Solution**: 
- Ensure `PhotonNetwork.AutomaticallySyncScene = true` is set
- Add the `PhotonSceneSyncHelper` script to your scene to automatically enable this

#### 2. **RPC Timing Issues**
**Problem**: RPC calls might not be processed by all clients before scene loading.
**Solution**: 
- The updated code now uses `RpcTarget.All` to ensure all clients receive the restart command
- Added delay in `RestartSceneWithDelay()` to ensure RPC processing

#### 3. **Master Client Authority**
**Problem**: Only the Master Client was calling `PhotonNetwork.LoadLevel()`.
**Solution**: 
- Now all clients call `PhotonNetwork.LoadLevel()` via RPC
- Added backup Master Client approach for redundancy

#### 4. **Network Latency**
**Problem**: Network delays can cause clients to miss the restart command.
**Solution**: 
- Added multiple restart attempts with different timing
- Master Client backup restart after 0.5 seconds

### Implementation Details

#### Updated RestartScene() Method
```csharp
private void RestartScene()
{
    if (PhotonNetwork.InRoom)
    {
        // Use RPC to ensure all clients restart the scene
        photonView.RPC("RPC_RestartScene", RpcTarget.All);
        
        // Also try the master client approach as backup
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(MasterClientRestartBackup());
        }
    }
    else
    {
        // Offline mode
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
```

#### RPC-Based Scene Restart
```csharp
[PunRPC]
private void RPC_RestartScene()
{
    Debug.Log("[MultiplayerDoor01Trigger] Restarting scene for all clients...");
    StartCoroutine(RestartSceneWithDelay());
}

private IEnumerator RestartSceneWithDelay()
{
    yield return null; // Wait a frame
    string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    
    if (PhotonNetwork.InRoom)
    {
        PhotonNetwork.LoadLevel(currentSceneName);
    }
    else
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName);
    }
}
```

### Additional Debugging Steps

#### 1. **Check Photon Settings**
- Verify `PhotonNetwork.AutomaticallySyncScene` is true
- Check if all clients are properly connected to the room
- Ensure the scene is in the Build Settings

#### 2. **Monitor Debug Logs**
The updated code includes comprehensive logging:
- Scene restart initiation
- RPC processing status
- Master Client backup triggers
- Scene loading confirmations

#### 3. **Use PhotonSceneSyncHelper**
Add this script to your scene to:
- Monitor Photon synchronization settings
- Log scene loading events
- Press F1 to check current sync status

#### 4. **Manual Force Sync**
If automatic sync fails, you can manually trigger:
```csharp
// Call this from any client to force scene sync
photonView.RPC("RPC_ForceSceneSync", RpcTarget.All);
```

### Testing the Fix

#### 1. **Test with 2 Players**
- Have Player 1 press Button 04 before Button 01
- Verify both clients restart the scene
- Check debug logs for restart confirmation

#### 2. **Test with 3 Players**
- Have any player press buttons in wrong order
- Verify all three clients restart simultaneously
- Monitor network latency effects

#### 3. **Test Network Conditions**
- Test with different network speeds
- Verify sync works with high latency
- Check behavior with packet loss

### Common Issues and Workarounds

#### Issue: Some clients don't restart
**Workaround**: The backup Master Client approach should handle this

#### Issue: Scene loads but players are desynchronized
**Workaround**: Ensure all players spawn at the same location after restart

#### Issue: RPC calls are missed
**Workaround**: The multiple restart attempts should catch missed calls

### Best Practices

1. **Always use RPC for scene changes** in multiplayer
2. **Include backup mechanisms** for critical operations
3. **Add comprehensive logging** for debugging
4. **Test with multiple network conditions**
5. **Verify Photon settings** before deployment

### Alternative Solutions

If the RPC approach still doesn't work, consider:

1. **Using Photon's Room Properties** to signal restart
2. **Implementing a custom scene sync system**
3. **Using Photon's Scene Management callbacks**
4. **Adding manual scene sync buttons for testing**
