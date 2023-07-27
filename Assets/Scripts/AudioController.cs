using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : Singleton<AudioController>
{
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip sound)
    {
        if (!sound) return;
       
        audioSource.clip = sound;
        audioSource.Play();
    }
}
