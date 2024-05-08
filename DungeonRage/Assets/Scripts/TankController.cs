using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
//using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TankController : MonoBehaviour
{
    public GameObject FloatingTextPrefab;
    // Target of the chase
    // (initialise via the Inspector Panel)
    public GameObject target;

    //Instacne of the HUD manager
    public HUDManager hud;

    bool idle = false;
    private bool isTouching;

    // Radius of the circle
    public float radius = 2f;

    // Number of points on circle's circumference
    int numPoints = 64;

    //Instance of steering basics
    SteeringBasics steeringBasics;

    // Reference to animator component
    Animator animator;

    //Boolean variable to control whether enemy is chasing or not
    //intialised to false so monster doesnt start chasing straight away
    bool isChasing = false;

    //The health of the enemy
    public float health = 200;

    //How much damage the enemy does to the player
    public float damage;

    public bool flip;

    private bool dying = false;

    [SerializeField] private AudioSource attackSound;

    // Start is called before the first frame update
    void Start()
    {
        //Find game object with player tag and set to target
        target = GameObject.FindGameObjectWithTag("Player");

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

        if (idle == false)
        {
            isChasing = targetCheck();
            if (isChasing)
            {
                idle = true;
            }

        }

        if (steeringBasics != null && isChasing)
        {
            animator.SetTrigger("walk");
            //Travel towards the target object at certain speed.

            Vector3 accel = steeringBasics.Arrive(target.transform.position);

            steeringBasics.Steer(accel);
        }

        float distanceToPlayer = Vector2.Distance(transform.position, target.transform.position);
        //if (distanceToPlayer <= 0.75f)
        //{
        //    animator.SetTrigger("attack");
        //}
        animator.SetFloat("attack1", distanceToPlayer);
        //animator.SetFloat("Speed", movement.sqrMagnitude);

    }

    public void chasingOff()
    {
        isChasing = false;

    }
    public void Attack()
    {
        //Vector2 pos = transform.localPosition;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        attackSound.Play();
        if (isTouching)
        {
            hud.DealDamage(damage);
        }

        //animator.SetTrigger("walk");

    }

    public void chasingOn()
    {
        isChasing = true;
    }
    private bool targetCheck()
    {
        // Compute the angle between two triangles in the cricle
        float delta = 2f * Mathf.PI / (float)(numPoints - 1);
        // Stat with angle of 0
        float alpha = 0f;

        // Specify the layer mast for ray casting - ray casting will
        // interact with layer 8 (player) and 9 (walls)
        int layerMask = 1 << 8 | 1 << 9;

        //Cast rays in circle around monster
        for (int i = 1; i <= numPoints; i++)
        {
            //Radius and alpha give a position of a point around
            //the circle in spherical coordinates 

            // Compute position x from spherical coordinates
            float x = radius * Mathf.Cos(alpha);
            // Compute position y from spherical coordinates
            float y = radius * Mathf.Sin(alpha);

            // Create a ray
            Vector2 ray = new Vector2(x, y);
            ray.x *= transform.lossyScale.x;
            ray.y *= transform.lossyScale.y;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, ray, ray.magnitude, layerMask);
            if (hit.collider != null && hit.collider.tag == "Player")
            {
                return true;
            }
            alpha += delta;
        }

        return false;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            isTouching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            isTouching = false;
        }
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
                    hud.GetExperience(30);
                    if (SceneManager.GetActiveScene().name == "EndlessMode")
                    {
                        ScoreManager.score += 300;
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
