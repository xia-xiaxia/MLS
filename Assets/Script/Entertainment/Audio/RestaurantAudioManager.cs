using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RestaurantAudioManager : Singleton<RestaurantAudioManager>
{
    private AudioSource audioSource;



    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void OnStartMusic()
    {
        audioSource.UnPause();
    }
    public void OnEndMusic()
    {
        audioSource.Pause();
    }
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
