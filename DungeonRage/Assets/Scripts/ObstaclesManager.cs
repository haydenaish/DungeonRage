using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build;
using UnityEngine;

public class ObstaclesManager : MonoBehaviour
{
    private void Update()
    {
        // Check if player is dashing
        if (PlayerMovement.isDashing == true ) // Added null check
        {
            // Turn off collider of the game object
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
        if(PlayerMovement.isDashing == false)
        {
            // Turn on collider of the game object
            gameObject.GetComponent<Collider2D>().enabled = true;
        }
    }
}
