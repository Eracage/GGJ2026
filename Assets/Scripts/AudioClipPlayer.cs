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
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
    public void PlayAudio()
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }
}
