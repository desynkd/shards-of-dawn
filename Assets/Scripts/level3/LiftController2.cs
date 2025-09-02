using UnityEngine;

public class LiftController : MonoBehaviour
{
    public Transform lift;
    public Vector3 upPosition;
    public Vector3 downPosition;
    public float moveSpeed = 2f;
    private bool isUp = false;
    private bool isMoving = false;

    public void MoveLift()
    {
        if (!isMoving)
        {
            StartCoroutine(MoveLiftCoroutine());
        }
    }

    private System.Collections.IEnumerator MoveLiftCoroutine()
    {
        isMoving = true;
        Vector3 target = isUp ? downPosition : upPosition;
        while (Vector3.Distance(lift.position, target) > 0.01f)
        {
            lift.position = Vector3.MoveTowards(lift.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        lift.position = target;
        isUp = !isUp;
        isMoving = false;
    }
}
