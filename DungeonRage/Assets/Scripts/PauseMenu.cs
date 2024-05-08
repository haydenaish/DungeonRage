using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject skillPointsPanel;
    public GameObject controlsPanel;
    // Start is called before the first frame update
    void Start()
    {
        //Default panel
        ShowPanel(skillPointsPanel);
    }
    public void ShowOptionsPanel()
    {
        ShowPanel(optionsPanel);
    }
    public void ShowSkillPointsPanel()
    {
        ShowPanel(skillPointsPanel);
    }
    public void ShowControlsPanel()
    {
        ShowPanel(controlsPanel);
    }
    void ShowPanel(GameObject panel)
    {
        //Disables panels
        optionsPanel.SetActive(false);
        skillPointsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        //Shows enabled panel
        panel.SetActive(true);
    }
}
