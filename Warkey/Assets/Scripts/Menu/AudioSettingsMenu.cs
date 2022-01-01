using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public void SetGameplayAudio(float gameplayVolume)
    {
        audioMixer.SetFloat("GameplayVolume", gameplayVolume);

    }

    public void SetMusic(float musicVolume)
    {
        audioMixer.SetFloat("MusicVolume", musicVolume);
    }

}
