using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] AudioClip btnClick;
    [SerializeField] AudioSource sfxAudioSource;

    public void PlayBtnClick()
    {
        sfxAudioSource.PlayOneShot(btnClick);
    }
}
