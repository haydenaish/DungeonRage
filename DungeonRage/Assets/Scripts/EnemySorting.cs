using UnityEngine;
using UnityEngine.Rendering;

public class EnemySorting : MonoBehaviour
{
    private GameObject player;
    private SortingGroup sortingGroup;

    private void Start()
    {
        player = GameObject.FindWithTag("Player"); 
        sortingGroup = GetComponent<SortingGroup>();
    }

    private void Update()
    {
        // Compare Y positions to determine whether the enemy is above or below the player.
        if (transform.position.y > player.transform.position.y)
        {
            sortingGroup.sortingOrder = player.GetComponent<SortingGroup>().sortingOrder - 1;
        }
        else
        {
            sortingGroup.sortingOrder = player.GetComponent<SortingGroup>().sortingOrder + 1;
        }
    }
}
