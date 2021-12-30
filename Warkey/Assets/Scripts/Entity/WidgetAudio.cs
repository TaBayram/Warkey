using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WidgetAudio : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip deathClip;


    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(WidgetAudio.Name name) {
        if(name == Name.death) {
            audioSource.clip = deathClip;
            audioSource.Play();
        }
    }

    public enum Name
    {
        death = 0,
        gotHit = 1,
        attacks = 2
    }
}
