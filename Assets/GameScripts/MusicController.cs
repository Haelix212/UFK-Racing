using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource backgroundMusic;

    void Start()
    {
        // Start playing music
        backgroundMusic.Play();
    }

    void Update()
    {
        // Example: Toggle music with the M key
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (backgroundMusic.isPlaying)
            {
                backgroundMusic.Pause();
            }
            else
            {
                backgroundMusic.UnPause();
            }
        }
    }
}
