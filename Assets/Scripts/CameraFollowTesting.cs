using UnityEngine;

public class CameraFollowTesting : MonoBehaviour
{
    [Tooltip("Drag the object you want the camera to follow here.")]
    public Transform target;

    public Vector3 offset = new Vector3(0, 0, -10);

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
