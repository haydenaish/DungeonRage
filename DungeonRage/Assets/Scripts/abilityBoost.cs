using System.Collections;
using UnityEngine;

public class abilityBoost : MonoBehaviour
{
    // Reference to the PlayerMovement script
    private PlayerMovement playerMovement;
    private SpellCast spellCast;

    // Original player stats
    private float originalDamageMultiplier;
    private float originalMoveSpeed;
    private float originalDashCooldown;
    private float originalSpellCooldown;

    // Boosted stats during Ability 2
    public float boostedDamageMultiplier = 1.5f;
    public float boostedMoveSpeed = 2f;
    public float reducedDashCooldown = 0.5f;
    public float reducedSpellCooldown = 0.1f;


    private void Start()
    {
        // Find the PlayerMovement script in the scene
        playerMovement = FindObjectOfType<PlayerMovement>();
        spellCast = FindObjectOfType<SpellCast>();

        // Store original player stats
        originalDamageMultiplier = playerMovement.damageMultiplier;
        originalMoveSpeed = playerMovement.baseMoveSpeed;
        originalDashCooldown = playerMovement.dashCooldown;
        originalSpellCooldown = spellCast.cooldownLength;
    }

    public void ActivateAbilityBoost()
    {
        // Apply boosted stats
        playerMovement.damageMultiplier *= boostedDamageMultiplier;
        playerMovement.baseMoveSpeed *= boostedMoveSpeed;
        playerMovement.dashCooldown *= reducedDashCooldown;
        spellCast.cooldownLength *= reducedSpellCooldown;

        // Debug the changes
        Debug.Log($"AbilityBoost: Activated. Damage Multiplier: {playerMovement.damageMultiplier}, Move Speed: {playerMovement.baseMoveSpeed}, Dash Cooldown: {playerMovement.dashCooldown}, Spell Cooldown: {spellCast.cooldownLength}");
    }

    public void DeactivateAbilityBoost()
    {
        // Restore original stats
        playerMovement.damageMultiplier = originalDamageMultiplier;
        playerMovement.baseMoveSpeed = originalMoveSpeed;
        playerMovement.dashCooldown = originalDashCooldown;
        spellCast.cooldownLength = originalSpellCooldown;

        // Debug the changes
        Debug.Log($"AbilityBoost: Activated. Damage Multiplier: {playerMovement.damageMultiplier}, Move Speed: {playerMovement.baseMoveSpeed}, Dash Cooldown: {playerMovement.dashCooldown}, Spell Cooldown: {spellCast.cooldownLength}");
    }
}
