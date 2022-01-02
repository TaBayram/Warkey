using UnityEngine;
using UnityEngine.SceneManagement;

public class FootSteps : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] grassWalkClips;
    [SerializeField]
    private AudioClip[] snowWalkClips;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void WalkingFootStep()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private void RunningFootStep()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        
        if (sceneName == "CampScene")
        {
            return grassWalkClips[UnityEngine.Random.Range(0, grassWalkClips.Length)];
        }
       else if (sceneName == "ForestScene")
        {
            return grassWalkClips[UnityEngine.Random.Range(0, grassWalkClips.Length)];
        }

        else if (sceneName == "WinterScene")
        {
            return snowWalkClips[UnityEngine.Random.Range(0, snowWalkClips.Length)];
        }
        else { 
           return null;
        }
    }
}