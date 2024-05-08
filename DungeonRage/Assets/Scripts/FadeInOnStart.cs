using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOnStart : MonoBehaviour
{
    public float fadeDuration = 2.0f; // Adjust the duration as needed
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();

        // Ensure the Image component is available
        if (image == null)
        {
            Debug.LogError("Image component not found on the object.");
            return;
        }

        // Start the fade-in process
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Color originalColor = image.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the alpha is set to 1 at the end to avoid potential issues
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        gameObject.SetActive(false);
    }
}
