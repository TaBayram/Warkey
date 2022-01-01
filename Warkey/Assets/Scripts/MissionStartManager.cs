using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionStartManager : MonoBehaviour
{
    private AudioSource audioSource;

    private bool hasPlayed;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }


    private void OnTriggerEnter(Collider other) {
        if (!hasPlayed) {
            audioSource.PlayDelayed(1f);
            hasPlayed = true;
        }
    }
}
