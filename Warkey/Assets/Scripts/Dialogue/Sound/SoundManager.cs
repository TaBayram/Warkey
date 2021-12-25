using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static AudioClip npcWelcome, npcWhere, npcGoodbye;
    static AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        npcWelcome = Resources.Load<AudioClip>("1");
        npcWhere = Resources.Load<AudioClip>("2");
        npcGoodbye = Resources.Load<AudioClip>("3");

        audioSource = GetComponent<AudioSource>();

        

      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "1":
                audioSource.PlayOneShot(npcWelcome);
                break;
            case "2":
                audioSource.PlayOneShot(npcWhere);
                break;
            case "3":
                audioSource.PlayOneShot(npcGoodbye);
                break;



        }
    }
}
