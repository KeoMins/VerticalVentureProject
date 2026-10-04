using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public float moveDistance = 3f; // Where the platform moves to
    public float moveSpeed = 2f; // How fast the platform moves

    Vector3 startPosition;
    Vector3 targetPosition;

    // Save where the platform starts 
    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition;
    }

    // Move the platform towards its target position
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // Tell the platform to move to its end position
    public void MoveToEnd()
    {
        targetPosition = startPosition + Vector3.right * moveDistance;
    }

    // Tell the platform to go back to its start position
    public void ReturnToStart()
    {
        targetPosition = startPosition;
    }
}
