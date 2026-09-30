using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private float moveSpeed = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

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
