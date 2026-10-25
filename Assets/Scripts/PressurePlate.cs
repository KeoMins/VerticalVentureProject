using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public MovePlatform platform;

    public float pressDepth = 0.05f; // How far the plate sinks when stepped on
    public float pressSpeed = 2f;    // How fast it sinks and rises

    int objectsOnPlate = 0; // Number of things on the plate

    Vector3 upPosition;
    Vector3 downPosition;

    void Start()
    {
        upPosition = transform.position;
        downPosition = upPosition + Vector3.down * pressDepth;
    }

    void Update()
    {
        Vector3 target = objectsOnPlate > 0 ? downPosition : upPosition;
        transform.position = Vector3.MoveTowards(transform.position, target, pressSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Either the player or the crate can activate the pressure plate
        if (other.attachedRigidbody != null)
        {
            objectsOnPlate++;
            platform.MoveToEnd();

            // Only click when the plate first goes down, not for every extra object
            if (objectsOnPlate == 1 && SoundEffects.Instance != null)
            {
                SoundEffects.Play(SoundEffects.Instance.plateDown);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody != null)
        {
            objectsOnPlate--;

            // Only move the platform back once the plate is empty
            if (objectsOnPlate == 0)
            {
                platform.ReturnToStart();
                if (SoundEffects.Instance != null)
                {
                    SoundEffects.Play(SoundEffects.Instance.plateUp);
                }
            }
        }
    }
}
