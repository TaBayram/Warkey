using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider soundSlider, musicSlider;

    private void Start() {
        soundSlider.value = PlayerPrefs.GetFloat("GameplayVolume", 0);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0);
    }

    public void SetGameplayAudio(float gameplayVolume)
    {
        gameplayVolume = Mathf.Log10(gameplayVolume);
        float volume = Mathf.Lerp(-80f, 10f, gameplayVolume+1);
        PlayerPrefs.SetFloat("GameplayVolume", volume);
        audioMixer.SetFloat("GameplayVolume", volume);

    }

    public void SetMusic(float musicVolume)
    {
        musicVolume = Mathf.Log10(musicVolume);
        float volume = Mathf.Lerp(-80f, 10f, musicVolume+1);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        audioMixer.SetFloat("MusicVolume", volume);
    }

}
