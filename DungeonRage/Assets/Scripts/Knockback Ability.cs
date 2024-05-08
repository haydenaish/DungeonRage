using System.Collections;
using UnityEngine;

public class KnockbackAbility : MonoBehaviour
{
    public float knockbackForce = 10f;
    public float knockbackRadius = 3f;
    public float tankForceMultiplier = 2;
    public float rangeForceMultiplier = 2;

    public int damageAmount = 50;

    public GameObject prefab;

    // Function to be called when using the knockback ability
    public void UseKnockbackAbility()
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
                // Check if the enemy is within the knockback radius
                if (distanceToEnemy <= knockbackRadius)
                {
                    // Apply knockback to the enemy with a uniform force
                    Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                    monsterController.Knockback(knockbackDirection, knockbackForce);

                    // Deal damage to the enemy
                    monsterController.TakeDamage(damageAmount);
                }
            }
            //If it is a tank enemy
            else if (tankController != null)
            {
                // Check if the enemy is within the knockback radius
                if (distanceToEnemy <= knockbackRadius)
                {
                    float force = knockbackForce * tankForceMultiplier;
                    // Apply knockback to the enemy with a uniform force
                    Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                    tankController.Knockback(knockbackDirection, force);

                    // Deal damage to the enemy
                    tankController.TakeDamage(damageAmount);
                }
            }
            //If its range enemy
            else
            {
                // Check if the enemy is within the knockback radius
                if (distanceToEnemy <= knockbackRadius)
                {
                    float force = knockbackForce * rangeForceMultiplier;
                    // Apply knockback to the enemy with a uniform force
                    Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                    rangeController.Knockback(knockbackDirection, force);

                    // Deal damage to the enemy
                    rangeController.TakeDamage(damageAmount);
                }
            }
        }
    }

    public void explosionAnim()
    {
        Vector2 pos = transform.position;

        pos.y -= 0.2f;
        GameObject spell = Instantiate(prefab, pos, Quaternion.identity);

        Animator animator = spell.GetComponent<Animator>();

        //Destroy(spell, animator.GetCurrentAnimatorStateInfo(0).length);
        StartCoroutine(WaitAndDestroy(animator.GetCurrentAnimatorStateInfo(0).length, spell, animator));
    }

    IEnumerator WaitAndDestroy(float duration, GameObject spellObject, Animator animator)
    {
        // Wait for the duration of the animation
        yield return new WaitForSeconds(duration);

        animator.enabled = false;
        // Wait for additional time before destroying (adjust the duration as needed)
        yield return new WaitForSeconds(2.0f);

        // Destroy the spell GameObject after waiting
        Destroy(spellObject);
    }
}
