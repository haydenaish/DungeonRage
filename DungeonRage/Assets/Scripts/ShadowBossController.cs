using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShadowBossController : MonoBehaviour
{
    public GameObject textPanel;
    // Start is called before the first frame update
    void Start()
    {
        textPanel.SetActive(false);
    }

    // Update is called once per frame
    public void TextPanelActive()
    {
        textPanel.SetActive(true);
        StartCoroutine(GameOver(10f));
    }

    public IEnumerator GameOver(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainMenu");
    }
}
