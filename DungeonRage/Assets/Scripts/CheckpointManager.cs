using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public GameObject player;
    private static Vector3 lastCheckpointPosition;
    public HUDManager hud;

    public Sprite activatedCheckpointSprite;
    private SpriteRenderer spriteRendererCheckpoint;

    void Start()
    {
        lastCheckpointPosition = player.transform.position;
    }

    void Update()
    {
        // Check for the shortcut key press
        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnPlayer();
        }
    }

    public void UpdateLastCheckpointPosition(Vector3 position)
    {
        lastCheckpointPosition = position;
    }

    public void ActivateCheckpoint(SpriteRenderer spriteRenderer)
    {
        spriteRendererCheckpoint = spriteRenderer;
        spriteRenderer.sprite = activatedCheckpointSprite;
    }

    public void RespawnPlayer()
    {
        if (player != null)
        {
            // Respawn the player at the last checkpoint
            player.transform.position = lastCheckpointPosition;

            // Reset the player's health
            hud.Heal(10000000);
        }
    }
}
