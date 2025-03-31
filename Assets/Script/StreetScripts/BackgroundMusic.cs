using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource.clip != null)  
        {
            audioSource.loop = true;  
            audioSource.Play();       
        }
        
    }
}
