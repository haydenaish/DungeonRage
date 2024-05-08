using System.Collections;
using UnityEngine;

public class TrackingFireball : MonoBehaviour
{
    private Transform target;  // Target to track
    public float force = 10f;  // Force applied to the fireball

    void Update()
    {
        // Check if the target is still valid
        if (target != null)
        {
            // Calculate the direction to the target
            Vector2 direction = (target.position - transform.position).normalized;

            // Apply force to move the fireball towards the target
            GetComponent<Rigidbody2D>().velocity = direction * force;
        }
        else
        {
            // If the target is no longer valid, despawn the fireball
            Destroy(gameObject);
        }
    }

    // Set the target for the fireball
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
