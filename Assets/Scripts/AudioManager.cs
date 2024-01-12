using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource EFAudioSource;
    public AudioClip[] HitDuckClips;
    public AudioClip ShowSceneClip;
    public AudioClip ReadyGoClip;
    public AudioClip ShotGunClip;
    public AudioClip MenuDuckClip;
    public AudioClip MenuDuckReadyClip;
    public AudioClip DuckGoBackClip;
    public AudioClip UnHitDuckClip;
    private void Awake()
    {
        Instance = this;
    }
    public void PlayOneShot(AudioClip audioClip)
    {
        EFAudioSource.PlayOneShot(audioClip);
    }
    public void PlayHitDuckClip()
    {
        PlayOneShot(HitDuckClips[Random.Range(0, HitDuckClips.Length)]);
    }
    public void PlayShowSceneClip()
    {
        PlayOneShot(ShowSceneClip);
    }
    public void PlayReadyGoClip()
    {
        PlayOneShot(ReadyGoClip);
    }
    public void PlayShotGunClip()
    {
        PlayOneShot(ShotGunClip);
    }
    public void PlayeMenuDuckClip()
    {
        PlayOneShot(MenuDuckClip);
    }
    public void PlayeMenuDuckReadyClip()
    {
        PlayOneShot(MenuDuckReadyClip);
    }
    public void PlayeDuckGoBackClip()
    {
        PlayOneShot(DuckGoBackClip);
    }
    public void PlayeUnHitDuckClip()
    {
        PlayOneShot(UnHitDuckClip);
    }
}
