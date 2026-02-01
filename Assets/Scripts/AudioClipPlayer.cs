using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioClipPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    public bool playOnStart = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (playOnStart)
            audioSource.PlayOneShot(audioClip);
    }
    public void PlayAudio()
    {
        audioSource.PlayOneShot(audioClip);
    }
}
