using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioClipPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    public bool playOnStart = false;

    public bool loop = false;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (playOnStart)
        {
            if (loop)
            {
                audioSource.loop = true;
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            audioSource.PlayOneShot(audioClip);
        }
    }
    public void PlayAudio()
    {
        audioSource.PlayOneShot(audioClip);
    }
}
