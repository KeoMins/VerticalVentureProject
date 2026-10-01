using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public MovePlatform platform;

    int objectsOnPlate = 0; // Number of things on the plate

    void OnTriggerEnter2D(Collider2D other)
    {
        // Either the player or the crate can activate the pressure plate (both have RigidBody2D)
        if (other.attachedRigidbody != null)
        {
            objectsOnPlate++;
            platform.MoveToEnd();
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
            }
        }
    }
}
