using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public SkillPointManager skillPointManager;
    public float baseDamage = 35f; // Base damage without multiplier
    public float damage;
    [SerializeField] private new ParticleSystem particleSystem = default;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Walls") || collision.gameObject.CompareTag("Shield") || collision.gameObject.CompareTag("Checkpoint"))
        {
            SpawnHitParticles(transform.position);
            DestroyFireball();
        }
        else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Tank") || collision.gameObject.CompareTag("Range") || collision.gameObject.CompareTag("Boss"))
        {
            // Access the PlayerMovement script
            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();

            // Check if the enemy has the MonsterController component
            MonsterController monsterController = collision.gameObject.GetComponent<MonsterController>();
            // Check if the enemy has the TankController component
            TankController tankController = collision.gameObject.GetComponent<TankController>();
            // Check if the enemy has the RangeController component
            RangeController rangeController = collision.gameObject.GetComponent<RangeController>();
            //Check if the boss has the bossController script
            BossController bossController = collision.gameObject.GetComponent <BossController>();

            float totalDamage = 0;

            if (playerMovement != null)
            {
                // Apply the damage multiplier
                totalDamage = damage * playerMovement.damageMultiplier;

                // Access and apply damage to the RageSystem
                RageSystem rageSystem = FindObjectOfType<RageSystem>();
                if (rageSystem != null)
                {
                    rageSystem.DealDamage(totalDamage);
                }

            }

            // If it's a basic monster
            if (monsterController != null)
            {
                monsterController.TakeDamage(totalDamage);
            }
            // If it's a tank enemy
            else if (tankController != null)
            {
                tankController.TakeDamage(totalDamage);
            }
            // If it's a ranged enemy
            else if (rangeController != null)
            {
                rangeController.TakeDamage(totalDamage);
            }
            // If its a boss
            else if(bossController != null)
            {
                //Debug.Log("test");
                bossController.TakeDamage(totalDamage);
            }

            //Debug.Log($"Total damage {totalDamage}");

            // Spawn hit particles at the collision point
            SpawnHitParticles(transform.position);

            // Destroy the fireball
            DestroyFireball();
        }
    }

    void SpawnHitParticles(Vector3 position)
    {
        // Instantiate the hit particles prefab at the collision point
        //Debug.Log("Spawning hit particles");

        if (particleSystem != null)
        {
            Instantiate(particleSystem, position, Quaternion.identity);

        }
    }

    void DestroyFireball()
    {
        Destroy(gameObject);
    }
}
