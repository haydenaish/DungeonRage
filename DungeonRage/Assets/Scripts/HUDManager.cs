using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


public class HUDManager : MonoBehaviour
{
    public Image healthBar;
    public Image experienceBar;
    public Image rageBar;
    public Button[] spellButtons;
    public GameObject pauseMenu;

    public static float baseHealthAmount = 100f;
    public static float healthAmount;
    private static float maxHealth;
    public float experienceAmount = 0f;
    public float rageAmount = 0f;

    public static bool isPaused;
    private bool isAbilityDraining = false; // Flag to track ability draining state

    private bool isInvincible = false;

    public SkillPointManager skillPointManager;
    public string resetSceneName = "EndlessMode";
    public string gameOverSceneName = "GameOverScene"; // Name of the GameOverScene

    public CheckpointManager checkPoint;
    public Animator animator;

    private bool isDying = false;

    // Start is called before the first frame update
    void Start()
    {
        SetPauseMenuActive(false);

        healthBar.fillAmount = baseHealthAmount / 100f;
        experienceBar.fillAmount = experienceAmount / 100f;
        rageBar.fillAmount = rageAmount / 100f;

        healthAmount = baseHealthAmount;
        maxHealth = healthAmount;

        skillPointManager = GameObject.FindObjectOfType<SkillPointManager>();
        foreach (var stat in skillPointManager.stats)
        {
            if (stat.statName == "Endurance")
            {
                stat.OnStatChanged += UpdateHealthAmount;
            }
        }

        for (int i = 0; i < spellButtons.Length; i++)
        {
            int index = i;
            spellButtons[i].onClick.AddListener(() => OnSpellButtonClicked(index));
        }

        // Subscribe to the DealDamageEvent
        RageSystem.DealDamageEvent += HandleDealDamageEvent;
    }

    void UpdateHealthAmount(float newEndurance)
    {
        Debug.Log($"New Endurance: {newEndurance}");

        // Calculate healthAmount based on baseHealthAmount and newEndurance
        maxHealth = baseHealthAmount + newEndurance + 10f;

        // Update the healthBar fill amount based on the calculated healthAmount
        healthBar.fillAmount = baseHealthAmount / maxHealth;

        Debug.Log($"Updated healthAmount: {maxHealth}");
    }

    public void DealDamage(float damage)
    {
        if (isInvincible)
        {
            return;
        }
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / maxHealth;

        if (healthAmount <= 0 && !isDying)
        {
            isDying = true;

            // Player is dead
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

            if (SceneManager.GetActiveScene().name == resetSceneName)
            {
                // Save the score before loading the game over scene
                PlayerPrefs.SetInt("PlayerScore", ScoreManager.score);
                PlayerPrefs.Save();
                ScoreManager.score = 0;
                SceneManager.LoadScene(gameOverSceneName);
            }
            TriggerDeathAnimation();
            Debug.Log("Player is dead");

        }
        else
        {
            return;
        }
    }


    private void TriggerDeathAnimation()
    {
        animator.SetTrigger("Death");
        StartCoroutine(RespawnAfterDeathAnimation());
    }

    private IEnumerator RespawnAfterDeathAnimation()
    {
        float deathAnimationDuration = 1.03f;
        animator.SetTrigger("Respawned");
        yield return new WaitForSeconds(deathAnimationDuration);

        checkPoint.RespawnPlayer();
        isDying = false;
    }

    public void SetInvincibilityState(bool isInvincible)
    {
        this.isInvincible = isInvincible;
    }

    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0f, maxHealth);

        healthBar.fillAmount = healthAmount / maxHealth;
    }

    public void GetExperience(float experience)
    {
        if (experienceAmount >= 100f)
        {
            experienceAmount = 0f;
            skillPointManager.LevelUp();
        }
        experienceAmount += experience;
        experienceAmount = Mathf.Clamp(experienceAmount, 0f, 100f);

        experienceBar.fillAmount = experienceAmount / 100f;
    }

    public void GetRage(float rage)
    {
        // Update rageAmount based on the gained rage, only if not in ability draining state
        if (!isAbilityDraining)
        {
            rageAmount += rage;
            rageAmount = Mathf.Clamp(rageAmount, 0f, 100f);
        }

        UpdateRageBar();
    }

    public void TogglePause()
    {
        isPaused = !Time.timeScale.Equals(0f);
        SetPauseMenuActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void SetPauseMenuActive(bool isActive)
    {
        pauseMenu.SetActive(isActive);
    }

    public void OnSpellButtonClicked(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 0:
                // Cast spell 1
                break;
            case 1:
                // Cast spell 2
                break;
            case 2:
                // Cast spell 3
                break;
            default:
                break;
        }
    }

    private void UpdateRageBar()
    {
        // Check if the rageBar object is not null and is active in the hierarchy
        if (rageBar != null && rageBar.gameObject != null && rageBar.gameObject.activeInHierarchy)
        {
            // Check if the Image component is not null
            Image imageComponent = rageBar.GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.fillAmount = rageAmount / 100f;
                //Debug.Log($"Updated Rage Bar. Rage Amount: {rageAmount}");
            }
            else
            {
                // Handle the case where Image component is null
                Debug.LogWarning("Image component is null. Unable to update fillAmount.");
            }
        }
        else
        {
            // Handle the case where rageBar object is null or not active
            Debug.LogWarning("RageBar object is null or not active. Unable to update fillAmount.");
        }
    }

    private void HandleDealDamageEvent(float damage)
    {
        if (damage < 0)
        {
            // Handle ability drain (negative damage)
            Debug.Log("Ability drain");
            rageAmount += damage;
            rageAmount = Mathf.Clamp(rageAmount, 0f, 100f);
        }
        else
        {
            // Handle normal damage
            rageAmount += damage * RageSystem.damageRageRatio;
            rageAmount = Mathf.Clamp(rageAmount, 0f, 100f);
            Debug.Log(rageAmount);
        }

        UpdateRageBar();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    // Add a public method to set the ability draining state from other scripts
    public void SetAbilityDrainingState(bool isDraining)
    {
        isAbilityDraining = isDraining;
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
        ScoreManager.score = 0;

    }
}
