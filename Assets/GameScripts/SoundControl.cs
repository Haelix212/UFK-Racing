using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class SoundControl : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private TextMeshProUGUI valueText;
    [SerializeField]
    private AudioMixMode mixMode;
    public GameObject musicPlayer;

    public void ChangeMasterSlider(float value)
    {
        valueText.SetText($"{(value * 10).ToString("N4")}");

        switch(mixMode)
        {
            case AudioMixMode.LinearAudioSourceVolume:
                audioSource.volume = value;
                break;
            case AudioMixMode.LinearMixerVolume:
                mixer.SetFloat("MasterVolume", (-80 + value * 100));
                break;
            case AudioMixMode.LogarithmicMixerVolume:
                mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
                break;
        }
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    public void ChangeMusicSlider(float value)
    {
        valueText.SetText($"{(value*10).ToString("N4")}");

        switch(mixMode)
        {
            case AudioMixMode.LinearAudioSourceVolume:
                audioSource.volume = value;
                break;
            case AudioMixMode.LinearMixerVolume:
                mixer.SetFloat("MusicVolume", (-80 + value * 100));
                break;
            case AudioMixMode.LogarithmicMixerVolume:
                mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
                break;
        }
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }
    public void ChangeSFXSlider(float value)
    {
        valueText.SetText($"{(value*10).ToString("N4")}");

        switch(mixMode)
        {
            case AudioMixMode.LinearAudioSourceVolume:
                audioSource.volume = value;
                break;
            case AudioMixMode.LinearMixerVolume:
                mixer.SetFloat("SFXVolume", (-80 + value * 100));
                break;
            case AudioMixMode.LogarithmicMixerVolume:
                mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
                break;
        }
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    private void Start()
    {
        musicPlayer = GameObject.Find("MusicPlayer");
        audioSource = musicPlayer.GetComponent<AudioSource>();
        mixer.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("Volume", 1) * 20));
        mixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("Volume", 1) * 20));
        mixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("Volume", 1) * 20));
    }
    
    public enum AudioMixMode
    {
        LinearAudioSourceVolume,
        LinearMixerVolume,
        LogarithmicMixerVolume
    }
}
