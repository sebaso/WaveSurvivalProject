using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeToBlack : MonoBehaviour
{
    public static FadeToBlack instance;
    public Image image;
    public float fadeSpeed = 5f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FadeFromBlack();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void FadeOut(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(1f, duration));
    }

    public void FadeIn(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(0f, duration));
    }

    public void FadetoBlack()
    {
        FadeOut(1f / fadeSpeed);
    }

    public void FadeFromBlack()
    {
        FadeIn(1f / fadeSpeed);
    }

    private IEnumerator FadeCoroutine(float targetAlpha, float duration)
    {
        if (image == null) yield break;

        float startAlpha = image.color.a;
        float timer = 0f;

        Debug.Log($"Starting Fade: current={startAlpha}, target={targetAlpha}, duration={duration}");

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            Color color = image.color;
            color.a = alpha;
            image.color = color;
            yield return null;
        }

        Color finalColor = image.color;
        finalColor.a = targetAlpha;
        image.color = finalColor;

        Debug.Log($"Fade Complete: alpha={image.color.a}");
    }
}
