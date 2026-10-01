using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    Vector3 startPosition;
    Vector3 targetPosition;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void MoveToEnd()
    {
        targetPosition = startPosition + Vector3.right * moveDistance;
    }

    public void ReturnToStart()
    {
        targetPosition = startPosition;
    }
}
