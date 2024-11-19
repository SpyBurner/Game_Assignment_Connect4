using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip ClickSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = ClickSound;
    }

    public void PlayClickSound()
    {
        audioSource.Play();
    }
}
