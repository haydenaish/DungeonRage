using UnityEngine;
using System.Collections;

public class RangedEnemyAI : MonoBehaviour
{
    private MovementAIRigidbody movementAI;
    public float targetDistance = 5f;
    public float stopDistance = 0.2f; 
    public float moveSpeed = 5f;

    void Start()
    {
        movementAI = GetComponent<MovementAIRigidbody>();
        StartCoroutine(RangedEnemyBehavior());
    }

    IEnumerator RangedEnemyBehavior()
    {
        while (true)
        {
            Vector3 playerDirection = (PlayerPosition() - movementAI.Position).normalized;

            // Calculate the distance from the player
            float distanceToPlayer = Vector3.Distance(movementAI.Position, PlayerPosition());

            if (distanceToPlayer > targetDistance + stopDistance)
            {
                // Calculate the desired position to maintain distance
                Vector3 desiredPosition = PlayerPosition() - playerDirection * targetDistance;

                // Calculate the desired velocity to reach the desired position
                Vector3 desiredVelocity = (desiredPosition - movementAI.Position).normalized * moveSpeed;

                movementAI.Velocity = desiredVelocity;
            }
            else if (distanceToPlayer < targetDistance - stopDistance)
            {
                movementAI.Velocity = Vector3.zero;
            }

            yield return null;
        }
    }

    Vector3 PlayerPosition()
    {
        return GameObject.FindGameObjectWithTag("Player").transform.position;
    }
}
