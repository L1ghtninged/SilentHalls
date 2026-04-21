using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource sfxSource;
    public AudioSource musicSource;

    public AudioClip attackClip;
    public AudioClip hitClip;
    public AudioClip deathClip;
    public AudioClip stepClip;
    public AudioClip fireClip;
    public AudioClip iceClip;
    public AudioClip healClip;
    public AudioClip levelClip;
    public AudioClip openDoorClip;
    public AudioClip ambienceLoop;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        PlayAmbience();
    }
    public void PlayAmbience()
    {
        musicSource.clip = ambienceLoop;
        musicSource.loop = true;
        musicSource.Play();
    }
    public void PauseSounds()
    {
        sfxSource.Pause();
        musicSource.Pause();
    }
    public void UnPauseSounds()
    {
        sfxSource.UnPause();
        musicSource.UnPause();
    }
    public void EnableSounds()
    {
        sfxSource.enabled = true;
        musicSource.enabled = true;
    }
    public void DisableSounds()
    {
        sfxSource.enabled = false;
        musicSource.enabled = false;
    }
    public void PlayAttack()
    {
        sfxSource.PlayOneShot(attackClip);
    }

    public void PlayHit()
    {
        sfxSource.PlayOneShot(hitClip);
    }

    public void PlayDeath()
    {
        sfxSource.PlayOneShot(deathClip);
    }

    public void PlayStep()
    {
        sfxSource.PlayOneShot(stepClip);
    }
    public void PlayFireMagic()
    {
        sfxSource.PlayOneShot(fireClip);
    }
    public void PlayIceMagic()
    {
        sfxSource.PlayOneShot(iceClip);
    }
    public void PlayHeal()
    {
        sfxSource.PlayOneShot(healClip);
    }
    public void PlayLevelUp()
    {
        sfxSource.PlayOneShot(levelClip);
    }
    public void PlayOpenDoor()
    {
        sfxSource.PlayOneShot(openDoorClip);
    }
}
