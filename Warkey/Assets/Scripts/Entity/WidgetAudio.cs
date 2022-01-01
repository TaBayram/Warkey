using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WidgetAudio : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip bornClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip hitClip;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(WidgetAudio.Name name) {
        if(name == Name.death && deathClip != null) {
            audioSource.clip = deathClip;
            audioSource.Play();
        }
        else if(name == Name.gotHit && hitClip != null) {
            audioSource.clip = hitClip;
            audioSource.Play();
        }
        else if (name == Name.attacks && attackClip != null) {
            audioSource.clip = attackClip;
            audioSource.Play();
        }
        else if (name == Name.born && bornClip != null) {
            audioSource.clip = bornClip;
            audioSource.Play();
        }
    }

    public enum Name
    {
        death = 0,
        gotHit = 1,
        attacks = 2,
        born = 3,
    }
}
