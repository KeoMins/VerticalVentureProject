using UnityEngine;
using UnityEngine.Serialization;

public class MovePlatform : MonoBehaviour
{
    [FormerlySerializedAs("moveDistance")]
    public float moveDistanceX = 3f; // How far the platform moves along the x-axis (negative = left)
    public float moveDistanceY = 0f; // How far the platform moves along the y-axis (negative = down)
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
        targetPosition = startPosition + new Vector3(moveDistanceX, moveDistanceY, 0f);
    }

    // Tell the platform to go back to its start position
    public void ReturnToStart()
    {
        targetPosition = startPosition;
    }
}
