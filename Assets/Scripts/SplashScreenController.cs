
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SplashScreenController : MonoBehaviour
{
    public string sceneAfterSplash = "MainMenu";
    public float splashDuration = 3.0f;
    public Image fadeImage;
    public AnimationCurve fadeGradient;
    private float timer = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Color color = fadeGradient.Evaluate(0) * Color.black;
        fadeImage.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        Color color = fadeGradient.Evaluate(timer / splashDuration) * Color.black;
        fadeImage.color = color;
        if (timer >= splashDuration)
        {
            SceneManager.LoadScene(sceneAfterSplash);
        }
    }
}
