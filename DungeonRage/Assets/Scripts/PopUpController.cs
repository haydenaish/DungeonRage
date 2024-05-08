using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpController : MonoBehaviour
{
    public GameObject popupPanel;

    private void Start()
    {
        popupPanel.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player entered collision range.");
            ShowPopUp(); // Show the pop-up
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            HidePopUp(); // Hide the pop-up
        }
    }

    private void ShowPopUp()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(true); // Activate the pop-up panel
        }
    }

    private void HidePopUp()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false); // Deactivate the pop-up panel
        }
    }
}