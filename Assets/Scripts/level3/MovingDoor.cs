using UnityEngine;

public class MovingDoor : MonoBehaviour
{
    public float moveDistance = 3f; // How far to move left/right
    public float moveSpeed = 2f;    // Movement speed

    private Vector3 startPos;
    private float timer;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime * moveSpeed;
        float offset = Mathf.Sin(timer) * moveDistance;
        transform.position = startPos + new Vector3(offset, 0, 0);
    }
}
