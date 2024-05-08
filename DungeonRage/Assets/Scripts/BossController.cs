using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossController : MonoBehaviour
{

    public GameObject FloatingTextPrefab;
    // Target of the chase
    // (initialise via the Inspector Panel)
    public GameObject target;

    //Instacne of the HUD manager
    public HUDManager hud;

    private bool flip;

    //Instances of the enemeis spawned
    public GameObject basicEnemy;
    public GameObject tankEnemy;
    public GameObject rangedEnemy;


    ////Instance of steering basics
    SteeringBasics steeringBasics;

    // Reference to animator component
    Animator animator;


    //Health of enemy
    public float health = 1000;
    private float maxHealth;
    private bool dying = false;
    private bool invincible = false;

    //prefab to spawn around boss
    public GameObject shieldPrefab;

    public GameObject ShadowBossPrefab;

    //List to hold all the different enemeis
    HashSet<GameObject> enemyTypes = new HashSet<GameObject>();

    //number of enemies spawned each routine
    public float maxEnemies = 5;

    //Time betweeen enemies spawning
    public float minSpawnInterval = 0.5f;
    public float maxSpawnInterval = 1f;

    //method to control the player entering room and boss starting to attack
    private static bool attacking = false;

    //Boolean to control whether the next attack should be spawning, set to true so anu
    private bool spawningNext = true;

    //Reference to the boss shot
    public GameObject projectilePrefab;

    //float for how many rounds the enemy shoots
    public float roundsShooting = 3;

    //Float to hold the force of the projectile.
    public float speed;

    private bool isChasing = false;

    public GameObject roomPrefab;
    private Collider2D roomCollider;

    public GameObject deathText;
    //Settor method for to start the boss attacking
    public static void setAttacking()
    {
        attacking = true;
        Debug.Log($"Test setter method attacking is {attacking}");
    }

    // Start is called before the first frame update
    void Start()
    {
        deathText.SetActive(false);
        //set the intial health to the max health
        maxHealth = health;

        //Add the intial basic enemy to the hashset
        enemyTypes.Add(basicEnemy);
        //Find game object with player tag and set to target
        target = GameObject.FindGameObjectWithTag("Player");

        // Initialise the reference to the Animator component
        animator = GetComponent<Animator>();

        steeringBasics = GetComponent<SteeringBasics>();

        ////Set the HUD to to the obejct with hud Tag
        hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManager>();

        roomCollider = roomPrefab.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dying)
        {
            StopAllCoroutines();
            return;
        }
        else
        {
            faceTarget();
            if (attacking)
            {
                Debug.Log("Stopping all routines");
                StopAllCoroutines();
                Debug.Log("Starting routine");
                StartCoroutine(AttackCoroutine());
                attacking = false;
                isChasing = true;
            }

            if (steeringBasics != null && isChasing)
            {
                //Travel towards the target object at certain speed.
                Vector3 accel = steeringBasics.Arrive(target.transform.position);

                steeringBasics.Steer(accel);
            }
        }
        //the attacking varaible is set the first time the player enters the room in room controller
        //Debug.Log(attacking == true);
    }

    public void TakeDamage(float damage)
    {
        if (invincible)
        {
            return;
        }
        else
        {
            health -= damage;

            if (dying)
            {
                return;
            }
            else
            {
                if (health <= 0)
                {
                    dying = true;
                    Debug.Log("dead");
                    animator.SetTrigger("dead");
                    //Destroy(gameObject);
                    return;
                }
                else if (health < 2f / 3f * maxHealth)
                {
                    enemyTypes.Add(tankEnemy);
                    return;
                }
                else if (health < 1f / 3f * maxHealth)
                {
                    enemyTypes.Add(rangedEnemy);
                    return;
                }

                if (FloatingTextPrefab)
                {
                    ShowFloatingText(Mathf.RoundToInt(damage).ToString());
                }
            }
        }
    }

    void ShowFloatingText(string text)
    {
        var go = Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
        var textMeshPro = go.GetComponent<TextMeshProUGUI>();

        if (textMeshPro == null)
        {
            // If TextMeshPro component is not found, try getting TextMeshPro - Text component
            var textMeshProText = go.GetComponent<TextMeshPro>();
            if (textMeshProText != null)
            {
                // Set text for TextMeshPro - Text component
                textMeshProText.text = text;
                if (flip)
                {
                    Vector3 scale = textMeshProText.transform.localScale;
                    scale.x *= -1f;
                    textMeshProText.transform.localScale = scale;
                }
            }
        }
    }

    private IEnumerator AttackCoroutine()
    {
        while (!dying)
        {
            yield return new WaitForSeconds(2);
            if (spawningNext)
            {
                yield return SpawnEnemies();
                spawningNext = false;
            }
            else
            {
                //animator.SetTrigger("charge");
                yield return Fire();
                spawningNext = true;
                animator.SetTrigger("decharge");
            }
        }
    }

    IEnumerator Fire()
    {
        animator.SetTrigger("charge");
        //Wait for charge animation to finsh as its only .4 seconds long
        yield return new WaitForSeconds(0.4f);
        List<GameObject> fireballs = new List<GameObject>();
        for (int rounds = 0; rounds <= roundsShooting; rounds++)
        {
            //Create fire fireballs
            for(int noBalls = 0; noBalls<=4; noBalls++)
            {
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

                // Get the Rigidbody2D component of the spell object
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

                // Ignore the collision between the projectile and the caster
                Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

                // Create a vector for the position of the projectile
                Vector2 pos = new Vector2(projectile.transform.position.x, projectile.transform.position.y);

                // Create a vector to hold the player's position
                Vector2 targetPos = new Vector2(target.transform.position.x, target.transform.position.y);

                // Calculate the direction from the player to the target
                Vector2 directionToPlayer = (targetPos - pos).normalized;

                // Calculate a random offset within a radius around the player
                float randomOffsetX = Random.Range(-0.2f, 0.2f);
                float randomOffsetY = Random.Range(-0.2f, 0.2f);

                // Apply the random offset to the direction
                Vector2 direction = directionToPlayer + new Vector2(randomOffsetX, randomOffsetY);
                rb.AddForce(direction * speed, ForceMode2D.Impulse);
            }

            // Add force in the direction
            yield return new WaitForSeconds(1.5f);
        }
    }

    IEnumerator SpawnEnemies()
    {
        //Stop enemie from accelerating to target
        isChasing = false;

        //Set rigid body speed to 0
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;

        //Create the shield game object
        GameObject sheild = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
        invincible = true;

        //Bounds of the room
        Bounds bounds = roomCollider.bounds;

        //Spawn enemeies
        foreach (var enemytype in enemyTypes)
        {
            float noSpawn = 0;
            while (noSpawn < maxEnemies)
            {
                Vector2 spawnPosition;
                while (true)
                {
                    // Generate a random angle in radians
                    float angle = Random.Range(0f, Mathf.PI * 2f); // 0 to 2π

                    // Calculate random position within the radius
                    float radius = Random.Range(0f, 2f); // Within 0 to 2 units
                    float spawnX = transform.position.x + radius * Mathf.Cos(angle);
                    float spawnY = transform.position.y + radius * Mathf.Sin(angle);

                    // Check if the spawn position is within the bounds of the room
                    //if (spawnX >= bounds.min.x && spawnX <= bounds.max.x && spawnY >= bounds.min.y && spawnY <= bounds.max.y)
                    //{
                    //    // Spawn position is within the bounds of the room
                    //    spawnPosition = new Vector2(spawnX, spawnY);
                    //    break; // Exit the loop
                    //}
                    spawnPosition = new Vector2(spawnX, spawnY);
                    break;
                }
                Instantiate(enemytype, spawnPosition, Quaternion.identity);
                noSpawn++;
                yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
            }
        }


        yield return new WaitForSeconds(1);
        Destroy(sheild);
        invincible = false;
        isChasing = true;

    }

    private Vector2 GetRandomSpawnPosition(Bounds bounds)
    {
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        Vector2 spawnPosition = new Vector2(randomX, randomY);
        return spawnPosition;
    }

    private void faceTarget()
    {
        Vector3 scale = transform.localScale;


        if (target.transform.position.x > transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x) * -1;
            //* (flip ? -1 : 1);
            flip = true;
        }
        else
        {
            scale.x = Mathf.Abs(scale.x);
            //*(flip ? -1 : 1);
            flip = false;
        }
        transform.localScale = scale;

    }

    public void ShowDeathText()
    {
        deathText.SetActive(true);
    }

    public void DeathSequence()
    {
        deathText.SetActive(false);
        Vector3 spawnPosition = transform.position + new Vector3(0f, 0.8f, -2f);
        Instantiate(ShadowBossPrefab, spawnPosition, Quaternion.identity);
    }

}
