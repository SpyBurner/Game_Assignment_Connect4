using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip ClickSound;

    private AudioSource audioSource;

    void Start()
    {
        // Tìm hoặc thêm Audio Source vào GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Liên kết Audio Clip với Audio Source
        audioSource.clip = ClickSound;
    }

    public void PlayClickSound()
    {
        // Phát âm thanh
        audioSource.Play();
    }
}
