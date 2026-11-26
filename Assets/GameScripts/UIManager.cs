using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI nowPlayingText;
    public TextMeshProUGUI instructionText;

    private void Start()
    {
        // Set the instruction text
        instructionText.text = "Press 'N' to change tracks";
        // Update the now playing text with the current track
        UpdateNowPlayingText(SoundtrackManager.CurrentTrackName);
    }

    public static void UpdateNowPlayingText(string trackName)
    {
        // Find all UIManagers in the scene and update their NowPlayingText
        foreach (UIManager uiManager in FindObjectsOfType<UIManager>())
        {
            uiManager.nowPlayingText.text = "Now Playing: " + trackName;
        }
    }
}
