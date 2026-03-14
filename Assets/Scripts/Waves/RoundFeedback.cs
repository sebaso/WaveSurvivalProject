using System.Collections;
using UnityEngine;

public class RoundFeedback : MonoBehaviour
{
    public static RoundFeedback instance;
    public AudioSource audioSource;
    public AudioClip roundStartSound;
    public Light lightningLight;
    public float lightningDuration = 1.0f;
    public float baseIntensity = 0f;
    public float maxIntensity = 10f; 
    public float fadeOutDuration = 2.0f;

    private Coroutine lightningCoroutine;

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

    public void PlayRoundStartFeedback()
    {
        if (audioSource != null && roundStartSound != null)
        {
            audioSource.PlayOneShot(roundStartSound);
        }

        if (lightningLight != null)
        {
            if (lightningCoroutine != null)
            {
                StopCoroutine(lightningCoroutine);
            }
            lightningCoroutine = StartCoroutine(LightningRoutine());
        }
    }

    private IEnumerator LightningRoutine()
    {
        float timer = 0f;

        lightningLight.gameObject.SetActive(true);
        while (timer < lightningDuration)
        {
            lightningLight.intensity = Random.Range(baseIntensity, maxIntensity);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            
            lightningLight.intensity = baseIntensity;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
            
            timer += Random.Range(0.1f, 0.25f);
        }
        lightningLight.intensity = maxIntensity;
        float fadeTimer = 0f;
        while (fadeTimer < fadeOutDuration)
        {
            fadeTimer += Time.deltaTime;
            lightningLight.intensity = Mathf.Lerp(maxIntensity, baseIntensity, fadeTimer / fadeOutDuration);
            yield return null;
        }

        lightningLight.intensity = baseIntensity;
    }
}
