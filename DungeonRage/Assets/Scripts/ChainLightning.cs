using System.Collections;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    private CircleCollider2D coll;
    public LayerMask enemyLayer;
    public float damage;

    public GameObject chainLightning;
    public GameObject beenStruck;

    public int amountToChain = 2;

    private GameObject startObject;
    private GameObject endObject;

    private Animator animations;

    public ParticleSystem Particles;

    private int spawns;

    // Start is called before the first frame update
    void Start()
    {
        if (amountToChain == 0) Destroy(gameObject);

        coll = GetComponent<CircleCollider2D>();

        animations = GetComponent<Animator>();

        Particles = GetComponent<ParticleSystem>();

        startObject = gameObject;

        spawns = 1;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyLayer == (enemyLayer | (1 << collision.gameObject.layer)) && !collision.GetComponentInChildren<Lightning>())
        {
            if (spawns != 0 && amountToChain > 0)
            {
                endObject = collision.gameObject;
                amountToChain -= 1;
                Instantiate(chainLightning, collision.gameObject.transform.position, Quaternion.identity);
                Instantiate(beenStruck, collision.gameObject.transform);

                // Deal damage to the enemy
                MonsterController enemy = collision.gameObject.GetComponent<MonsterController>();
                // Check if the enemy has the TankController component
                TankController tankController = collision.gameObject.GetComponent<TankController>();
                // Check if the enemy has the RangeController component
                RangeController rangeController = collision.gameObject.GetComponent<RangeController>();
                if (enemy != null)
                {
                    // Access the PlayerMovement script
                    PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
                    if (playerMovement != null)
                    {
                        // Apply the damage multiplier
                        float totalDamage = damage * playerMovement.damageMultiplier;

                        // Deal damage to the enemy
                        enemy.TakeDamage(totalDamage);

                        // Access and apply damage to the RageSystem
                        RageSystem rageSystem = FindObjectOfType<RageSystem>();
                        if (rageSystem != null)
                        {
                            rageSystem.DealDamage(totalDamage);
                        }

                        // Show debug log for damage dealt
                        Debug.Log($"Lightning dealt {totalDamage} damage to {enemy.gameObject.name}");
                    }
                }
                else if (tankController != null)
                {
                    // Access the PlayerMovement script
                    PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
                    // Apply the damage multiplier
                    float totalDamage = damage * playerMovement.damageMultiplier;

                    // Deal damage to the enemy
                    tankController.TakeDamage(totalDamage);

                    // Access and apply damage to the RageSystem
                    RageSystem rageSystem = FindObjectOfType<RageSystem>();
                    if (rageSystem != null)
                    {
                        rageSystem.DealDamage(totalDamage);
                    }

                    // Show debug log for damage dealt
                    Debug.Log($"Lightning dealt {totalDamage} damage to {enemy.gameObject.name}");
                }else
                {
                    // Access the PlayerMovement script
                    PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
                    // Apply the damage multiplier
                    float totalDamage = damage * playerMovement.damageMultiplier;

                    // Deal damage to the enemy
                    rangeController.TakeDamage(totalDamage);

                    // Access and apply damage to the RageSystem
                    RageSystem rageSystem = FindObjectOfType<RageSystem>();
                    if (rageSystem != null)
                    {
                        rageSystem.DealDamage(totalDamage);
                    }

                }
            

            animations.StopPlayback();
                coll.enabled = false;
                spawns--;
                Particles.Play();

                // Emit particles between start and end positions
                EmitParticlesBetween(startObject.transform.position, endObject.transform.position);

                // Destroy the clone if it's not the original chain lightning
               if (!IsOriginalChainLightning())
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    // Emit particles between start and end positions
    private void EmitParticlesBetween(Vector3 startPosition, Vector3 endPosition)
    {
        var emitParams = new ParticleSystem.EmitParams();
        emitParams.position = startPosition;
        Particles.Emit(emitParams, 1);

        emitParams.position = endPosition;
        Particles.Emit(emitParams, 1);

        emitParams.position = (startPosition + endPosition) / 2;
        Particles.Emit(emitParams, 1);
    }

    // Check if the current instance of ChainLightning is the original one
    private bool IsOriginalChainLightning()
    {
        return gameObject == startObject;
    }
}
