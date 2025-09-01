using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Photon Networking/Photon Rigidbody2D View")]
public class PhotonRigidbody2DView : MonoBehaviourPun, IPunObservable
{
    private Rigidbody2D m_Body;

    // Network received values
    private Vector2 m_NetworkPosition;
    private float m_NetworkRotation;
    private Vector2 m_NetworkVelocity;
    private float m_NetworkAngularVelocity;

    // Smoothing settings
    [HideInInspector]
    public bool m_SynchronizeVelocity = true;
    [HideInInspector]
    public bool m_SynchronizeAngularVelocity = false;
    [HideInInspector]
    public bool m_TeleportEnabled = true;
    [HideInInspector]
    public float m_TeleportIfDistanceGreaterThan = 3.0f;
    [HideInInspector]
    public float m_SmoothingSpeed = 10f;
    [HideInInspector]
    public bool m_UseLerp = true;

    // Interpolation
    private Vector2 m_TargetPosition;
    private float m_TargetRotation;
    private bool m_ReceivedNetworkUpdate = false;

    public void Awake()
    {
        this.m_Body = GetComponent<Rigidbody2D>();
        this.m_NetworkPosition = this.m_Body.position;
        this.m_NetworkRotation = this.m_Body.rotation;
        this.m_TargetPosition = this.m_Body.position;
        this.m_TargetRotation = this.m_Body.rotation;
    }

    public void FixedUpdate()
    {
        if (!this.photonView.IsMine && m_ReceivedNetworkUpdate)
        {
            // Handle teleportation for large discrepancies
            if (m_TeleportEnabled && Vector2.Distance(m_Body.position, m_NetworkPosition) > m_TeleportIfDistanceGreaterThan)
            {
                m_Body.position = m_NetworkPosition;
                m_Body.rotation = m_NetworkRotation;
                m_TargetPosition = m_NetworkPosition;
                m_TargetRotation = m_NetworkRotation;
                return;
            }

            // Smooth movement towards target
            if (m_UseLerp)
            {
                m_Body.position = Vector2.Lerp(m_Body.position, m_TargetPosition, Time.fixedDeltaTime * m_SmoothingSpeed);
                m_Body.rotation = Mathf.LerpAngle(m_Body.rotation, m_TargetRotation, Time.fixedDeltaTime * m_SmoothingSpeed);
            }
            else
            {
                m_Body.position = Vector2.MoveTowards(m_Body.position, m_TargetPosition, Time.fixedDeltaTime * m_SmoothingSpeed * 5f);
                m_Body.rotation = Mathf.MoveTowardsAngle(m_Body.rotation, m_TargetRotation, Time.fixedDeltaTime * m_SmoothingSpeed * 100f);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send position and rotation
            stream.SendNext(this.m_Body.position);
            stream.SendNext(this.m_Body.rotation);

            // Send velocity if enabled
            if (this.m_SynchronizeVelocity)
            {
                stream.SendNext(this.m_Body.linearVelocity);
            }

            // Send angular velocity if enabled
            if (this.m_SynchronizeAngularVelocity)
            {
                stream.SendNext(this.m_Body.angularVelocity);
            }
        }
        else
        {
            // Receive position and rotation
            this.m_NetworkPosition = (Vector2)stream.ReceiveNext();
            this.m_NetworkRotation = (float)stream.ReceiveNext();

            // Calculate target position with lag compensation
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            
            if (this.m_SynchronizeVelocity)
            {
                Vector2 velocity = (Vector2)stream.ReceiveNext();
                this.m_NetworkVelocity = velocity;
                
                // Predict position based on velocity and lag
                this.m_TargetPosition = this.m_NetworkPosition + velocity * lag;
            }
            else
            {
                this.m_TargetPosition = this.m_NetworkPosition;
            }

            if (this.m_SynchronizeAngularVelocity)
            {
                float angularVelocity = (float)stream.ReceiveNext();
                this.m_NetworkAngularVelocity = angularVelocity;
                
                // Predict rotation based on angular velocity and lag
                this.m_TargetRotation = this.m_NetworkRotation + angularVelocity * lag;
            }
            else
            {
                this.m_TargetRotation = this.m_NetworkRotation;
            }

            m_ReceivedNetworkUpdate = true;
        }
    }
}
