using UnityEngine;

public class Floating : MonoBehaviour
{
    [Header("Random Float Settings")]
    public float moveRange = 50f;
    public float moveSpeed = 20f;
    
    private Vector3 targetPosition;
    private Vector3 originalPosition;
    
    void Start()
    {
        originalPosition = transform.position;
        SetNewTarget();
    }
    
    void Update()
    {
        // Move towards target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        // When close to target, set a new one
        if (Vector3.Distance(transform.position, targetPosition) < 5f)
        {
            SetNewTarget();
        }
    }
    
    void SetNewTarget()
    {
        float randomX = originalPosition.x + Random.Range(-moveRange, moveRange);
        float randomY = originalPosition.y + Random.Range(-moveRange, moveRange);
        targetPosition = new Vector3(randomX, randomY, originalPosition.z);
    }
}