using UnityEngine;
using TMPro; 

public class PlayerAbilities : MonoBehaviour
{
    public TextMeshProUGUI rageText; 
    public float rageGainRate = 20f; 

    private float currentRage = 0f;
    private bool isAbility2Active = false;
    private bool isAbility3Active = false;

    private float ability2DrainRate = 10f; 
    private float ability3DrainRate = 15f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GainRagePoints();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && currentRage >= 25f)
        {
            UseAbility1();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && currentRage >= 60f)
        {
            UseAbility2();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && currentRage >= 100f)
        {
            UseAbility3();
        }

        rageText.text = $"Rage: {Mathf.Round(currentRage)}";

        if (isAbility2Active)
        {
            DrainRagePoints(ability2DrainRate * Time.deltaTime);
            if (currentRage <= 0f)
            {
                DeactivateAbility2();
            }
        }

        if (isAbility3Active)
        {
            DrainRagePoints(ability3DrainRate * Time.deltaTime);
            if (currentRage <= 0f)
            {
                DeactivateAbility3();
            }
        }
    }

    void GainRagePoints()
    {
        currentRage = Mathf.Clamp(currentRage + rageGainRate, 0f, 100f);
    }

    void DrainRagePoints(float amount)
    {
        currentRage = Mathf.Clamp(currentRage - amount, 0f, 100f);
    }

    void UseAbility1()
    {
        Debug.Log("Using Ability 1: Knocking back enemy sprite");

        DrainRagePoints(25f);
    }

    void UseAbility2()
    {
        Debug.Log("Using Ability 2: Increased damage and attack speed");

        isAbility2Active = true;
    }

    void DeactivateAbility2()
    {
        isAbility2Active = false;
        Debug.Log("Ability 2 deactivated");
    }

    void UseAbility3()
    {
        Debug.Log("Using Ability 3: More powerful attack");

        isAbility3Active = true;
    }

    void DeactivateAbility3()
    {
        isAbility3Active = false;
        Debug.Log("Ability 3 deactivated");
    }
}
