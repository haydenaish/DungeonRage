using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public Vector2 teleportPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Get the player's current position
            Vector2 newPosition = other.transform.position;

            // Set the x and y components of the player's position to the teleport position
            newPosition.x = teleportPosition.x;
            newPosition.y = teleportPosition.y;

            // Teleport the player to the specified position
            other.transform.position = newPosition;
        }
    }
}
