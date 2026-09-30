using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private MovePlatform platform;

    private readonly HashSet<Collider2D> occupants = new HashSet<Collider2D>();

    private void Awake()
    {
        if (platform == null)
        {
            Debug.LogError("PressurePlate: no MovePlatform assigned in the Inspector.", this);
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsActivator(other)) return;

        occupants.Add(other);
        platform.MoveToEnd();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!occupants.Remove(other)) return;

        if (occupants.Count == 0)
        {
            platform.ReturnToStart();
        }
    }

    // The player and the crate both have a Rigidbody2D
    private bool IsActivator(Collider2D other)
    {
        return other.attachedRigidbody != null;
    }
}
