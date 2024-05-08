using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{

    //public SkillPointManager skillPointManager;
    public float damage;

    //Instacne of the HUD manager
    public HUDManager hud;


    private void Start()
    {
        ////Set the HUD to to the obejct with hud Tag
        hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            hud.DealDamage(damage);
            Destroy(gameObject);
        }
    }

}
