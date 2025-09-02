using UnityEngine;

public class LiftController : MonoBehaviour
{
    public float moveDistance = 3f; // Distance to move up and down
    public float moveSpeed = 2f;    // Speed of movement
    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingUp = true;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * moveDistance;
    }

    void Update()
    {
        if (movingUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                movingUp = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPos) < 0.01f)
            {
                movingUp = true;
            }
        }
    }
}
