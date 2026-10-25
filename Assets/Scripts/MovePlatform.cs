using UnityEngine;
using UnityEngine.Serialization;

public class MovePlatform : MonoBehaviour
{
    [FormerlySerializedAs("moveDistance")]
    public float moveDistanceX = 3f; // How far the platform moves along the x-axis
    public float moveDistanceY = 0f; // How far the platform moves along the y-axis
    public float moveSpeed = 2f; // How fast the platform moves

    Vector3 startPosition;
    Vector3 targetPosition;
    AudioSource movingLoop;

    // Save where the platform starts
    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition;

        if (SoundEffects.Instance != null)
        {
            movingLoop = SoundEffects.AddLoop(gameObject, SoundEffects.Instance.platformMoving);
        }
    }

    // Move the platform towards its target position
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Play the moving sound only while the platform is travelling
        SoundEffects.SetLoop(movingLoop, transform.position != targetPosition);
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
