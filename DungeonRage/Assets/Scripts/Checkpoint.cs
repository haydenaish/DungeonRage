using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public CheckpointManager checkpointManager;
    public SpriteRenderer spriteRenderer;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            checkpointManager.UpdateLastCheckpointPosition(transform.position);
            //Debug.Log("Checkpoint reached!");

            checkpointManager.ActivateCheckpoint(spriteRenderer);
        }
    }
}
