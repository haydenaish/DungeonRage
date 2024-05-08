using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RangeController : MonoBehaviour
{

    // Target of the chase
    // (initialise via the Inspector Panel)
    public GameObject target;

    //Instacne of the HUD manager
    public HUDManager hud;

    private bool flip;
    //Instance of steering basics
    SteeringBasics steeringBasics;

    // Reference to animator component
    Animator animator;

    //Boolean variable to control whether enemy is chasing or not
    //intialised to false so monster doesnt start chasing straight away
    bool isChasing = true;

    //Health of enemy
    public float health = 150;

    //after enemy is on screen shots at random interval between these two values
    public float minShootInterval;
    public float maxShootInterval;

    //varaible to keep track so the first time the enemy is on screen it starts shooting
    private bool isEnemyVisible = false;

    //Referene to the camera to check if the enemy is on screen
    private Camera mainCamera;

    //Reference to the enemy shot
    public GameObject projectilePrefab;

    //Float to hold the force of the projectile.
    public float speed;

    public GameObject FloatingTextPrefab;
    // Start is called before the first frame update

    [SerializeField] private AudioSource audioSource ;

    private bool dying = false;
    void Start()
    {
        mainCamera = Camera.main;

        //Find game object with player tag and set to target
        target = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // Initialise the reference to the Animator component
        animator = GetComponent<Animator>();

        steeringBasics = GetComponent<SteeringBasics>();

        ////Set the HUD to to the obejct with hud Tag
        hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManager>();
    }

    // Update is called once per frame
    void Update()
    {
        faceTarget();

        if(!isEnemyVisible && IsVisibleOnScreen())
        {
            isEnemyVisible = true;
            Debug.Log("starting routine");
            StartCoroutine(AttackPlayerCoroutine());
        }

        if (steeringBasics != null && isChasing)
        {
            //Travel towards the target object at certain speed.
            Vector3 accel = steeringBasics.Arrive(target.transform.position);

            steeringBasics.Steer(accel);
        }
    }

    IEnumerator AttackPlayerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minShootInterval, maxShootInterval));
            animator.SetTrigger("attack");
            yield return new WaitForSeconds(1f);
            audioSource.Play();
        }
    }

    private bool IsVisibleOnScreen()
    {
        Vector3 playerViewportPos = mainCamera.WorldToViewportPoint(transform.position);
        return playerViewportPos.x >= 0 && playerViewportPos.x <= 1 &&
               playerViewportPos.y >= 0 && playerViewportPos.y <= 1;

    }

    public void shot()
    {
        // Instantiate the enemyProjectile object
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Get the Rigidbody2D component of the spell object
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        //Ignore the collision between the 
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        //Create a vector for the position of the projectile
        Vector2 pos = new Vector2(projectile.transform.position.x, projectile.transform.position.y);

        //Create a 2d vector to hold the targets position
        Vector2 targetPos = new Vector2(target.transform.position.x, target.transform.position.y);

        // Calculate the direction from the SpellCastPos to the mouse position
        Vector2 direction = (targetPos - pos).normalized;

        // Calculate the rotation angle in radians
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the projectile to face the direction of the mouse
        projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Add force in the direction of the mouse
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
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

    public void TakeDamage(float damage)
    {
        health -= damage;
        ScoreManager.score += (int)damage;
        if (dying)
        {
            return;
        }
        else
        {
            if (health <= 0)
            {
                animator.SetTrigger("death");
                dying = true;
                if (dying)
                {
                    hud.GetExperience(15);
                    if (SceneManager.GetActiveScene().name == "EndlessMode")
                    {
                        ScoreManager.score += 150;
                    }
                }
            }
            //Trigger floating text
            if (FloatingTextPrefab)
            {
                ShowFloatingText(Mathf.RoundToInt(damage).ToString());
            }
        }

    }

    public void death()
    {
        Destroy(gameObject);
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
    public void Knockback(Vector2 direction, float force)
    {
        // Stop chasing
        isChasing = false;

        // Apply knockback force
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        // Start a coroutine to simulate a knockback duration
        StartCoroutine(KnockbackDuration());
    }

    private IEnumerator KnockbackDuration()
    {
        // Wait for a short duration to simulate knockback effect
        yield return new WaitForSeconds(1f);

        // Resume chasing after knockback duration
        isChasing = true;
        //anim.SetTrigger("run");
    }

}
