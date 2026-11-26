using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    static string previouslyCalledScreen = "StartPage";
    public static bool IsRacing;

    public void LoadMain()
    {
        SceneManager.LoadScene("StartPage");
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("SelectGameMode");  // replace "GameScene" with the name of your game scene
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("CreditsPage");
    }

    public void LoadSettings()
    {
        SceneManager.LoadScene("SettingsPage");
    }

    public void LoadCarsRacing()
    {
        IsRacing = true;
        SceneManager.LoadScene("SelectCar");  // replace "GameScene" with the name of your game scene
    }

    public void LoadCarsDriving()
    {
        IsRacing = false;
        SceneManager.LoadScene("SelectCar");  // replace "GameScene" with the name of your game scene
    }

    public void LoadTrack()
    {
        previouslyCalledScreen = "SelectCar";
        SceneManager.LoadScene("SelectTrack");
    }

    public void LoadRace()
    {
        previouslyCalledScreen = "SelectTrack";

        if (TrackCarousel.ChosenTrack == 0)
        {
            // track 1 - nascar
            SceneManager.LoadScene("Nascar_Track");
        }
        else if (TrackCarousel.ChosenTrack == 1)
        {
            // track 2 - figure 8
            SceneManager.LoadScene("8_Track");
        }
        else if (TrackCarousel.ChosenTrack == 2)
        {
            // track 3 - big knot
            SceneManager.LoadScene("Knot_Track");
        }
        else
        {
            // track 4 - big track
            SceneManager.LoadScene("Big_Track");
        }

    }

    public void RestartGame()
    {
        previouslyCalledScreen = "StartPage";
        SceneManager.LoadScene("SelectGameMode");  // replace "GameScene" with the name of your game scene

    }
    public void GoBack()
    {
        SceneManager.LoadScene(previouslyCalledScreen);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
