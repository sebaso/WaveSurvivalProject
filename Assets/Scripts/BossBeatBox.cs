using UnityEngine;

public class BossBeatBox : MonoBehaviour
{
    public AudioSource audioSource;

    private void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        BossAI.OnBossDefeated += StopMusic;
    }

    private void OnDisable()
    {
        BossAI.OnBossDefeated -= StopMusic;
    }

    private void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Boss Music Stopped.");
        }
    }
}
