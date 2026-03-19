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
    public void FadetoBlack()
    {
        StartCoroutine(FadeToBlackCoroutine());
    }
    public void FadeFromBlack()
    {
        StartCoroutine(FadeFromBlackCoroutine());
    }
    private IEnumerator FadeToBlackCoroutine()
    {
        Color color = image.color;
        while (color.a < 1)
        {
            color.a += fadeSpeed * Time.deltaTime;
            image.color = color;
            yield return null;
        }
    }
    private IEnumerator FadeFromBlackCoroutine()
    {
        Color color = image.color;
        color.a = 1;
        image.color = color;
        while (color.a > 0)
        {
            color.a -= fadeSpeed * Time.deltaTime;
            image.color = color;
            yield return null;
        }
    }
}
