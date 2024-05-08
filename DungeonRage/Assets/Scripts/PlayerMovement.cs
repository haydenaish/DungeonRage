using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float baseMoveSpeed = 3f;
    public float moveSpeed;
    private bool isFacingRight = true;
    public Rigidbody2D rb;
    public Animator animator;

    public float baseDamageMultiplier = 1.0f;
    public float damageMultiplier;

    private Vector2 movement;
    public static bool isDashing = false;
    private float dashTime = 0.2f;
    private float dashDistance = 2f;
    public float dashCooldown = 3f; // Cooldown time for the dash 
    private float playerOffset = 0.1f; 
    private float lastDashTime = 0f;

    private float cooldownTimer = 0.0f;
    public Image imageCooldown;

    public float cooldownAnimationTime;
    private Coroutine cooldownCoroutine;

    public static bool isMoving;
    [SerializeField] private new ParticleSystem particleSystem = default;

    public SkillPointManager skillPointManager;

    [SerializeField] private AudioSource dashSoundEffect;
    [SerializeField] private AudioSource runSoundEffect;
    [SerializeField] private bool isPlayingRunSound = false;

    private float oldAgility = 0f;

    private void Start()
    {

        imageCooldown.fillAmount = 0f;

        moveSpeed = baseMoveSpeed;

        cooldownAnimationTime = dashCooldown;

        skillPointManager = GameObject.FindObjectOfType<SkillPointManager>();

        foreach(var stat in skillPointManager.stats)
        {
            if (stat.statName == "Agility")
            {
               stat.OnStatChanged += UpdateMoveSpeed;
            }
            if(stat.statName == "Intelligence")
            {
                stat.OnStatChanged += UpdateDamageMultiplier;
            }
        }
    }

    void UpdateDamageMultiplier(float intelligence)
    {
        damageMultiplier = baseDamageMultiplier + intelligence * 0.01f;
    }
    void UpdateMoveSpeed(float newAgility)
    {
        //Agility cap
        if(newAgility > 50 && newAgility > oldAgility)
        {
            oldAgility = newAgility;
            return;
        } 
        else if (newAgility < 50)
        {
            oldAgility = newAgility;
            moveSpeed = baseMoveSpeed + newAgility * 0.05f;
        }
       
    }
    void Update()
    {
        if (!isDashing)
        {
            // Input for movement
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            // Normalizing vector for diagonal movement
            Vector2 inputVector = new Vector2(horizontalInput, verticalInput);
            movement = inputVector.magnitude == 0 ? Vector2.zero : inputVector.normalized;
            
            if (Input.GetKeyDown(KeyCode.Space) && Time.time - lastDashTime > dashCooldown)
            {
                SpawnHitParticles(transform.position);
                dashSoundEffect.Play();
                StartCoroutine(Dash());
                lastDashTime = Time.time;
                imageCooldown.fillAmount = 1.0f;
            }

            if(Time.time - lastDashTime < dashCooldown)
            {
                cooldownTimer = Time.time - lastDashTime;
                imageCooldown.fillAmount = 1 - cooldownTimer / dashCooldown;

                if(cooldownTimer > 0f && cooldownCoroutine == null)
                {
                    cooldownCoroutine = StartCoroutine(CooldownFillAnimation());
                }
            }
            else
            {
                imageCooldown.fillAmount = 0.0f;

                if(cooldownCoroutine != null)
                {
                    StopCoroutine(cooldownCoroutine);
                    cooldownCoroutine = null;
                }
            }

            if(movement.magnitude > 0)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }   

            if (isMoving && !isPlayingRunSound)
            {
                PlayRunEffect();
                isPlayingRunSound = true;
            }
            else if (!isMoving && isPlayingRunSound)
            {
                StopRunEffect();
                isPlayingRunSound = false;
            }
            

            // Update animator
            animator.SetFloat("Speed", movement.sqrMagnitude);

            // Flip character if needed
            Flip();
        }
    }
    private void PlayRunEffect()
    {
        //Debug.Log("Playing run sound effect");
        runSoundEffect.Play();
    }
    private void StopRunEffect()
    {
        //Debug.Log("Stopping run sound effect");
        runSoundEffect.Stop();
    }

    IEnumerator CooldownFillAnimation()
    {
        float startTime = Time.time;
        float elapsedTime = 0f;
        float startFill = imageCooldown.fillAmount;

        while (elapsedTime < cooldownAnimationTime)
        {
            imageCooldown.fillAmount = Mathf.Lerp(startFill, 0f, elapsedTime / cooldownAnimationTime);
            elapsedTime = Time.time - startTime;
            yield return null;
        }

        imageCooldown.fillAmount = 0f;
        cooldownCoroutine = null;
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            // Move the character
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public IEnumerator Dash()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("Tank");
        GameObject[] range = GameObject.FindGameObjectsWithTag("Range");

        GameObject[] allEnemies = new GameObject[tanks.Length + enemies.Length + range.Length];
        enemies.CopyTo(allEnemies, 0);
        tanks.CopyTo(allEnemies, enemies.Length);
        range.CopyTo(allEnemies, tanks.Length + enemies.Length);

        isDashing = true;

        foreach (GameObject enemy in allEnemies)
        {
            if (enemy != null)
            {
                Collider2D playerCollider = rb.GetComponent<Collider2D>();
                Collider2D enemyCollider = enemy.GetComponent<Collider2D>();

                if (playerCollider != null && enemyCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, enemyCollider, true);
                }
            }
        }

        Vector2 dashDirection = movement.magnitude == 0 ? Vector2.zero : movement.normalized;
        Vector2 dashTarget = (Vector2)transform.position + dashDirection * dashDistance;

        RaycastHit2D hit = Physics2D.Raycast(rb.position, dashDirection, dashDistance, LayerMask.GetMask("Walls"));

        if (hit.collider != null && hit.collider.CompareTag("Walls"))
        {
            Vector2 closestPoint = hit.collider.ClosestPoint(rb.position);
            dashTarget = closestPoint + (closestPoint - (Vector2)transform.position).normalized * playerOffset;
        }

        float startTime = Time.time;
        float elapsedTime = 0f;

        while (elapsedTime < dashTime)
        {
            rb.MovePosition(Vector2.Lerp(rb.position, dashTarget, elapsedTime / dashTime));
            elapsedTime = Time.time - startTime;
            yield return null;
        }

        isDashing = false;

        foreach (GameObject enemy in allEnemies)
        {
            if (enemy != null)
            {
                Collider2D playerCollider = rb.GetComponent<Collider2D>();
                Collider2D enemyCollider = enemy.GetComponent<Collider2D>();

                if (playerCollider != null && enemyCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, enemyCollider, false);
                }
            }
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

    private void Flip()
    {
        // Flip character sprite based on movement direction
        if (isFacingRight && movement.x < 0f || !isFacingRight && movement.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
