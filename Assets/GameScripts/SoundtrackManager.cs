using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SoundtrackManager : MonoBehaviour
{
    private static SoundtrackManager instance;

    public AudioSource audioSource;
    public AudioClip[] soundtracks;
    private int currentTrackIndex;
    private string[] trackList = { 
        "Nujabes - Light on the Land", "Flying Lotus - Using What You Got", 
        "Flume - Ezra", "Queens of the Stone Age - Regular John", 
        "Travis Scott - Highest in the Room (Ocarina of Time Remix)", 
        "Kavinsky - Nightcall", "J Dilla - Mash", "Covet - Glimmer",
        "Vince Staples - Etouffee", "Ariana Grande - the boy is mine"};

    public TextMeshProUGUI nowPlayingText;

    public static string CurrentTrackName => instance != null ? instance.trackList[instance.currentTrackIndex] : "Unknown";

    void Awake()
    {
        // Check if an instance of SoundtrackManager already exists
        if (instance == null)
        {
            // If not, set this as the instance and mark it to not be destroyed
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource.loop = true;
            StartCoroutine(PreloadAudioClips());
            // currentTrackIndex = Random.Range(0, soundtracks.Length);
            currentTrackIndex = 5; // I like Nightcall being the default main menu music so 
            PlayCurrentTrack();
        }
        else
        {
            Debug.Log("Destroying soundtrack");
            // If an instance already exists, destroy this one
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Update the now playing text
        UpdateNowPlayingText();
    }

    void Update()
    {
        // Example: Cycle to the next track with the N key
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextTrack();
        }
    }

    public void NextTrack()
    {
        int prevIndex = currentTrackIndex;
        while (prevIndex == currentTrackIndex) {
            currentTrackIndex = Random.Range(0, soundtracks.Length);
        }
        PlayCurrentTrack();
        UIManager.UpdateNowPlayingText(CurrentTrackName); // Update UI in all scenes
    }

    private void PlayCurrentTrack()
    {
        audioSource.clip = soundtracks[currentTrackIndex];
        audioSource.Play();
    }

    private IEnumerator PreloadAudioClips()
    {
        foreach (AudioClip clip in soundtracks)
        {
            // Create a temporary AudioSource to preload the clip
            AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();
            tempAudioSource.clip = clip;
            tempAudioSource.volume = 0f; // Mute the sound
            tempAudioSource.Play();
            yield return new WaitForSeconds(0.1f); // Wait a bit to ensure the clip is loaded
            tempAudioSource.Stop();
            Destroy(tempAudioSource);
        }
    }

    private void UpdateNowPlayingText()
    {
        nowPlayingText.text = "Now Playing: " + trackList[currentTrackIndex];
    }
}
