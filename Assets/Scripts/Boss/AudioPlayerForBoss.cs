using UnityEngine;

public class AudioPlayerForBoss : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] footSteps;
    public AudioClip[] slamSounds;
    public AudioClip throwWindUp;
    public AudioClip throwRelease;
    public AudioClip deathSound;


    public void PlaySound(AudioClip clip)
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            print("idiot");
        }
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

}
