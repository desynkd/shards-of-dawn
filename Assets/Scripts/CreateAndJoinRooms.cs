using UnityEngine;
using TMPro;    
using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;
    [Header("UI References")]
    public TextMeshProUGUI errorMessageText; // Reference to error message UI element
    
    // Auto-find error message UI if not assigned
    private ErrorMessageUI errorMessageUI;

    void Start()
    {
        // Auto-find error message UI if not assigned
        if (errorMessageText == null)
        {
            errorMessageUI = FindObjectOfType<ErrorMessageUI>();
            if (errorMessageUI != null)
            {
                // Get the error text component from the ErrorMessageUI
                var errorTextComponent = errorMessageUI.GetComponentInChildren<TextMeshProUGUI>();
                if (errorTextComponent != null)
                {
                    errorMessageText = errorTextComponent;
                }
            }
        }
    }

    public void CreateRoom()
    {
        Debug.Log("[CreateAndJoinRooms] CreateRoom called. Input: " + createInput.text);
        
        // Clear previous error messages
        ClearErrorMessage();
        
        if (string.IsNullOrEmpty(createInput.text.Trim()))
        {
            ShowErrorMessage("Please enter a team name.");
            return;
        }
        
        if (createInput.text.Length < 3)
        {
            ShowErrorMessage("Team name must be at least 3 characters long.");
            return;
        }
        
        if (createInput.text.Length > 20)
        {
            ShowErrorMessage("Team name must be less than 20 characters.");
            return;
        }
        
        Debug.Log("[CreateAndJoinRooms] Creating room: " + createInput.text);
        PhotonNetwork.CreateRoom(createInput.text.Trim());
    }

    public void JoinRoom()
    {
        Debug.Log("[CreateAndJoinRooms] JoinRoom called. Input: " + joinInput.text);
        
        // Clear previous error messages
        ClearErrorMessage();
        
        if (string.IsNullOrEmpty(joinInput.text.Trim()))
        {
            ShowErrorMessage("Please enter a room name to join.");
            return;
        }
        
        Debug.Log("[CreateAndJoinRooms] Joining room: " + joinInput.text);
        PhotonNetwork.JoinRoom(joinInput.text.Trim());
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("[CreateAndJoinRooms] OnJoinedRoom called. IsMasterClient: " + PhotonNetwork.IsMasterClient);
        // Clear any error messages when successfully joining
        ClearErrorMessage();
        
        // Let the MasterClient own scene loading. Others will auto-sync.
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[CreateAndJoinRooms] Loading WaitingRoom scene as MasterClient.");
            PhotonNetwork.LoadLevel("WaitingRoom");
        }
        // Non-master: do not load a scene here when AutomaticallySyncScene is true.
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"[CreateAndJoinRooms] OnCreateRoomFailed called. ReturnCode: {returnCode}, Message: {message}");
        
        string errorMessage = "Failed to create team.";
        
        // Handle specific error codes using Photon ErrorCode constants
        switch (returnCode)
        {
            case Photon.Realtime.ErrorCode.GameIdAlreadyExists: // 32766
                errorMessage = "Team name already exists. Please choose a different name.";
                break;
            case Photon.Realtime.ErrorCode.GameFull: // 32765
                errorMessage = "Team is full.";
                break;
            case Photon.Realtime.ErrorCode.GameClosed: // 32764
                errorMessage = "Team is closed.";
                break;
            case Photon.Realtime.ErrorCode.ServerFull: // 32762
                errorMessage = "Server is full. Please try again later.";
                break;
            case Photon.Realtime.ErrorCode.UserBlocked: // 32761
                errorMessage = "You are blocked from creating teams.";
                break;
            case Photon.Realtime.ErrorCode.NoRandomMatchFound: // 32760
                errorMessage = "No random match found.";
                break;
            case Photon.Realtime.ErrorCode.GameDoesNotExist: // 32758
                errorMessage = "Team does not exist.";
                break;
            case Photon.Realtime.ErrorCode.MaxCcuReached: // 32757
                errorMessage = "Maximum concurrent users reached.";
                break;
            case Photon.Realtime.ErrorCode.InvalidRegion: // 32756
                errorMessage = "Invalid region.";
                break;
            case Photon.Realtime.ErrorCode.InvalidAuthentication: // 32767
                errorMessage = "Authentication failed.";
                break;
            case Photon.Realtime.ErrorCode.AuthenticationTicketExpired: // 32753
                errorMessage = "Authentication ticket expired.";
                break;
            default:
                errorMessage = $"Failed to create team: {message}";
                break;
        }
        
        ShowErrorMessage(errorMessage);
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"[CreateAndJoinRooms] OnJoinRoomFailed called. ReturnCode: {returnCode}, Message: {message}");
        
        string errorMessage = "Failed to join team.";
        
        // Handle specific error codes using Photon ErrorCode constants
        switch (returnCode)
        {
            case Photon.Realtime.ErrorCode.GameIdAlreadyExists: // 32766
                errorMessage = "Team already exists.";
                break;
            case Photon.Realtime.ErrorCode.GameFull: // 32765
                errorMessage = "Team is full.";
                break;
            case Photon.Realtime.ErrorCode.GameClosed: // 32764
                errorMessage = "Team is closed.";
                break;
            case Photon.Realtime.ErrorCode.ServerFull: // 32762
                errorMessage = "Server is full. Please try again later.";
                break;
            case Photon.Realtime.ErrorCode.UserBlocked: // 32761
                errorMessage = "You are blocked from joining teams.";
                break;
            case Photon.Realtime.ErrorCode.NoRandomMatchFound: // 32760
                errorMessage = "No random match found.";
                break;
            case Photon.Realtime.ErrorCode.GameDoesNotExist: // 32758
                errorMessage = "Team does not exist.";
                break;
            case Photon.Realtime.ErrorCode.MaxCcuReached: // 32757
                errorMessage = "Maximum concurrent users reached.";
                break;
            case Photon.Realtime.ErrorCode.InvalidRegion: // 32756
                errorMessage = "Invalid region.";
                break;
            case Photon.Realtime.ErrorCode.InvalidAuthentication: // 32767
                errorMessage = "Authentication failed.";
                break;
            case Photon.Realtime.ErrorCode.AuthenticationTicketExpired: // 32753
                errorMessage = "Authentication ticket expired.";
                break;
            default:
                errorMessage = $"Failed to join team: {message}";
                break;
        }
        
        ShowErrorMessage(errorMessage);
    }
    
    private void ShowErrorMessage(string message)
    {
        if (errorMessageText != null)
        {
            errorMessageText.text = message;
            errorMessageText.color = Color.red;
            errorMessageText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError($"[CreateAndJoinRooms] Error: {message}");
        }
    }
    
    private void ClearErrorMessage()
    {
        if (errorMessageText != null)
        {
            errorMessageText.text = "";
            errorMessageText.gameObject.SetActive(false);
        }
    }
}
