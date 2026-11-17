using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip DigitalBreak;
    public AudioClip Fire;
    public AudioClip Reflect;
    public AudioClip EnemyExplode;
    public AudioClip Die;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void PlayDigitalBreak()
    {
        audioSource.PlayOneShot(DigitalBreak, 1.0f);
    }

    public void PlayDie()
    {
        audioSource.PlayOneShot(Die, 1.0f);
    }

    public void PlayFire()
    {
        audioSource.PlayOneShot(Fire, 1.0f);
    }

    public void PlayReflect()
    {
        audioSource.PlayOneShot(Reflect, 1.0f);
    }

    public void PlayEnemyExplode()
    {
        audioSource.PlayOneShot(EnemyExplode, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
