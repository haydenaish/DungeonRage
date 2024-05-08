using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class SpellCast : MonoBehaviour
{

    public Transform SpellCastPos;
    //public GameObject SpellTypePrefab;
    public GameObject FireballPrefab;
    public GameObject LightningPrefab;
    public Animator animator;
    public GameObject Spell;

    private int spellType = 1;

    public bool spellReady = true;
    private float Timer = 0f;
    public float cooldownLength = 2f;

    private bool LightningReady = true;
    public float LightningCooldown = 8f;
    private float LastLightningTime = 0f;

    public Image imageCooldown;
    public Image lightningCooldownImage;

    public float cooldownAnimationTime;
    private Coroutine cooldownCoroutine;
    private float lastFireballTime = 0f;

    public Image fireballSelected; 
    public Image lightningSelected;

    public float force = 10f;

    public GameObject originalFireballPrefab; 
    public GameObject currentFireballPrefab;

    private RageSystem rageSystem;

    [SerializeField] private AudioSource fireballSound;
    [SerializeField] private AudioSource LightningSound;

    private void Start()
    {
        imageCooldown.fillAmount = 0f;
        lightningCooldownImage.fillAmount = 0f;
        cooldownAnimationTime = cooldownLength;
        currentFireballPrefab = originalFireballPrefab;
        rageSystem = FindObjectOfType<RageSystem>();
        fireballSelected.gameObject.SetActive(true);
        lightningSelected.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (!HUDManager.isPaused)
        {
            if (!spellReady)
            {
                Timer += Time.deltaTime;
                if (Timer > cooldownLength)
                {
                    spellReady = true;
                    Timer = 0;
                }
            }
            if (!LightningReady)
            {
                float remainingCooldown = Time.time - LastLightningTime;
                if (remainingCooldown < LightningCooldown)
                {
                    lightningCooldownImage.fillAmount = 1 - remainingCooldown / LightningCooldown;
                }
                else
                {
                    lightningCooldownImage.fillAmount = 0.0f;
                    LightningReady = true; // Reset spell readiness when cooldown is complete
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                spellType = 1;
                lightningSelected.gameObject.SetActive(false);
                fireballSelected.gameObject.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                spellType = 2;
                fireballSelected.gameObject.SetActive(false);
                lightningSelected.gameObject.SetActive(true);
            }

            if (spellReady && Input.GetButtonDown("Fire1") && spellType == 1)
            {
                //fireballSound.Play();
                if (PlayerMovement.isMoving)
                {
                    animator.SetTrigger("shot1");
                    fireballSound.Play();

                }
                else
                {
                    animator.SetTrigger("shot2");
                    fireballSound.Play();
                }

                imageCooldown.fillAmount = 1f; // Set fill amount to full at the start of cooldown
                spellReady = false;
                lastFireballTime = Time.time;

                // Start the cooldown fill animation coroutine
                cooldownCoroutine = StartCoroutine(CooldownFillAnimation());
            }
            if (LightningReady && Input.GetButtonDown("Fire1") && spellType == 2)
            {
                //fireballSound.Play();
                if (PlayerMovement.isMoving)
                {
                    animator.SetTrigger("shot1");
                    LightningSound.Play();

                }
                else
                {
                    animator.SetTrigger("shot2");
                    LightningSound.Play();
                }
                lightningCooldownImage.fillAmount = 1f; // Set fill amount to full at the start of cooldown
                LightningReady = false;
                LastLightningTime = Time.time;
                // Start the cooldown fill animation coroutine
                cooldownCoroutine = StartCoroutine(LightningCooldownFillAnimation());
            }
            if(!LightningReady)
            {
                float lightremainingCooldown = Time.time - LastLightningTime;
                if (lightremainingCooldown < LightningCooldown)
                {
                    lightningCooldownImage.fillAmount = 1 - lightremainingCooldown / LightningCooldown;
                }
                else
                {
                    lightningCooldownImage.fillAmount = 0.0f;
                    LightningReady = true; // Reset spell readiness when cooldown is complete
                }
            }

            if (!spellReady)
            {
                float remainingCooldown = Time.time - lastFireballTime;
                if (remainingCooldown < cooldownLength)
                {
                    imageCooldown.fillAmount = 1 - remainingCooldown / cooldownLength;
                }
                else
                {
                    imageCooldown.fillAmount = 0.0f;
                    spellReady = true; // Reset spell readiness when cooldown is complete
                }
            }
        }
    }

    IEnumerator CooldownFillAnimation()
    {
        float startTime = Time.time;
        float elapsedTime = 0f;
        float startFill = 1f; // Start fill amount at full

        while (elapsedTime < cooldownLength)
        {
            imageCooldown.fillAmount = Mathf.Lerp(startFill, 0f, elapsedTime / cooldownLength);
            elapsedTime = Time.time - startTime;
            yield return null;
        }

        imageCooldown.fillAmount = 0f;
    }
    IEnumerator LightningCooldownFillAnimation()
    {
        float startTime = Time.time;
        float elapsedTime = 0f;
        float startFill = 1f; // Start fill amount at full

        while (elapsedTime < LightningCooldown)
        {
            lightningCooldownImage.fillAmount = Mathf.Lerp(startFill, 0f, elapsedTime / LightningCooldown);
            elapsedTime = Time.time - startTime;
            yield return null;
        }

        lightningCooldownImage.fillAmount = 0f;
    }





    void CastSpell()
    {

        //animator.SetTrigger("shot1");

        // Find the mouse position
        //mousePos = PlayerMovement.cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Instantiate the spell object
        switch (spellType)
        {
            case 2:
                //Instantiate the lightning spell only when spellType is 3 (lightning spell)
                Spell = Instantiate(LightningPrefab, SpellCastPos.position, Quaternion.identity);
                break;
            default:
                Spell = Instantiate(rageSystem.GetCurrentFireballPrefab(), SpellCastPos.position, Quaternion.identity);
                // Ignore collisions between the player and the spell
                Physics2D.IgnoreCollision(Spell.GetComponent<Collider2D>(), GetComponent<Collider2D>());

                //Create a vector for the position of the spell
                Vector2 pos = new Vector2(SpellCastPos.position.x, SpellCastPos.position.y);

                // Calculate the direction from the SpellCastPos to the mouse position
                Vector2 direction = (mousePos - pos).normalized;

                // Get the Rigidbody2D component of the spell object
                Rigidbody2D rb = Spell.GetComponent<Rigidbody2D>();

                // Calculate the rotation angle in radians
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Rotate the fireball to face the direction of the mouse
                Spell.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                // Add force in the direction of the mouse
                rb.AddForce(direction * force, ForceMode2D.Impulse);
                break;
        }
    }
}
