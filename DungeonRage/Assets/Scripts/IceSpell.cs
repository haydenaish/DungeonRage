using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpell : MonoBehaviour
{
    public SkillPointManager skillPointManager;
    public float baseDamage = 35f; // Base damage without multiplier
    public float damage;

    public float FreezeRadius = 1f;

    //public GameObject prefab;
    

    public void UseFreezeAbility()
    {
            // Find all objects with an Enemy tag
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            // Find all objects with an Tank tag
            GameObject[] tanks = GameObject.FindGameObjectsWithTag("Tank");
            // Find all objects with an Range tag
            GameObject[] range = GameObject.FindGameObjectsWithTag("Range");

            // Create a new array with a size that accommodates both arrays
            GameObject[] allEnemies = new GameObject[tanks.Length + enemies.Length + range.Length];

            // Copy the elements of the enemies array to the new array, starting after the tanks
            enemies.CopyTo(allEnemies, 0);

            // Copy the elements of the tanks array to the new array
            tanks.CopyTo(allEnemies, enemies.Length);

            // Copy the elements of the ranges array to the new array, starting after the tanks and enemies
            range.CopyTo(allEnemies, tanks.Length + enemies.Length);

            foreach (GameObject enemy in allEnemies)
            {
                // Check if the enemy has the MonsterController component
                MonsterController monsterController = enemy.GetComponent<MonsterController>();
                // Check if the enemy has the TankController component
                TankController tankController = enemy.GetComponent<TankController>();
                // Check if the enemy has the RangeController component
                RangeController rangeController = enemy.GetComponent<RangeController>();

                // Calculate the distance between the player and the enemy
                float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

                Debug.Log(rangeController == null);
                //If it is a monster enemy
                if (monsterController != null)
                {

                //MonsterController collisionEnemy = collision.gameObject.GetComponent<MonsterController>();
                Vector2 FreezeDirection = (enemy.transform.position - transform.position).normalized;
                monsterController.Freeze(FreezeDirection);

                if (enemy != null)
                {
                    // Access the PlayerMovement script
                    PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();

                    if (playerMovement != null)
                    {
                        // Apply the damage multiplier
                        float totalDamage = damage * playerMovement.damageMultiplier;

                        // Deal damage to the enemy
                        monsterController.TakeDamage(totalDamage);

                        // Access and apply damage to the RageSystem
                        RageSystem rageSystem = FindObjectOfType<RageSystem>();
                        if (rageSystem != null)
                        {
                            rageSystem.DealDamage(totalDamage);
                        }

                        // Show debug log for damage dealt
                        Debug.Log($"IceSpell dealt {totalDamage} damage to {enemy.gameObject.name}");
                    }
                }

                Destroy(gameObject);
            }
        }
    }
}