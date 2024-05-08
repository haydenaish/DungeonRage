using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballNoRB : MonoBehaviour
{
    public int baseDamage = 30; // Base damage without multiplier
    [SerializeField] private new ParticleSystem particleSystem = default;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Walls") || collision.gameObject.CompareTag("Shield"))
        {
            SpawnHitParticles(transform.position);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Tank") || collision.gameObject.CompareTag("Range"))
        {
            // Access the corresponding enemy script based on the tag
            MonsterController enemy = collision.gameObject.GetComponent<MonsterController>();
            TankController tankEnemy = collision.gameObject.GetComponent<TankController>();
            RangeController rangeEnemy = collision.gameObject.GetComponent<RangeController>();

            if (enemy != null || tankEnemy != null || rangeEnemy != null)
            {
                // Access the PlayerMovement script
                PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();

                if (playerMovement != null)
                {
                    // Choose the appropriate enemy type and apply the damage multiplier
                    int totalDamage = 0;
                    if (enemy != null)
                    {
                        totalDamage = Mathf.RoundToInt(baseDamage * playerMovement.damageMultiplier);
                        enemy.TakeDamage(totalDamage);
                    }
                    else if (tankEnemy != null)
                    {
                        totalDamage = Mathf.RoundToInt(baseDamage * playerMovement.damageMultiplier);
                        tankEnemy.TakeDamage(totalDamage);
                    }
                    else if (rangeEnemy != null)
                    {
                        totalDamage = Mathf.RoundToInt(baseDamage * playerMovement.damageMultiplier);
                        rangeEnemy.TakeDamage(totalDamage);
                    }

                    // Access and apply damage to the RageSystem
                    RageSystem rageSystem = FindObjectOfType<RageSystem>();
                    if (rageSystem != null)
                    {
                        rageSystem.DealDamage(totalDamage);
                    }

                    // Show debug log for damage dealt
                    Debug.Log($"Fireball dealt {totalDamage} damage to {collision.gameObject.name}");
                }
            }

            SpawnHitParticles(transform.position);


            Destroy(gameObject);
        }
    }

    void SpawnHitParticles(Vector3 position)
    {
        // Instantiate the hit particles prefab at the collision point
        Debug.Log("Spawning hit particles");

        if (particleSystem != null)
        {
            Instantiate(particleSystem, position, Quaternion.identity);
        }
    }
}
