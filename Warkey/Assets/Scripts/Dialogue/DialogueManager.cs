using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    public NPC npc;

    bool isTalking = false;

    float distance;
    float curResponseTracker = 0;

    public GameObject player;
    public GameObject dialogueUI;

    public Text npcName;
    public Text npcDialogueBox;
    public Text playerResponse;

    


    // Start is called before the first frame update
    void Start()
    {
        dialogueUI.SetActive(false);

    }

    void OnMouseOver()
    {
        distance = Vector3.Distance(player.transform.position, this.transform.position);
        if(distance<=2.5f)
        {
            if(Input.GetAxis("Mouse ScrollWheel")>0f)
            {
                curResponseTracker++;
                if(curResponseTracker >= npc.playerDialogue.Length - 1)
                {
                    curResponseTracker = npc.playerDialogue.Length - 1;
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                curResponseTracker--;
                if (curResponseTracker < 0)
                {
                    curResponseTracker = 0;
                }
            }
            //trigger dialogue
            if(Input.GetKeyDown(KeyCode.E) && isTalking==false)
            {
                StartConversation();
            }
            else if(Input.GetKeyDown(KeyCode.E) && isTalking==true)
            {
                EndDialogue();
            }

            //for (int i = 0; i < curResponseTracker.Length; i++)
            //{

            //}
            if (curResponseTracker == 0 && npc.playerDialogue.Length > 0)
            {
                playerResponse.text = npc.playerDialogue[0];
                if (Input.GetMouseButtonDown(0))
                {
                    SoundManager.PlaySound("1");
                    npcDialogueBox.text = npc.dialogue[1];
                }
            }
            else if (curResponseTracker == 1 && npc.playerDialogue.Length > 0)
            {
                playerResponse.text = npc.playerDialogue[1];
                if (Input.GetMouseButtonDown(0))
                {
                    SoundManager.PlaySound("2");
                    npcDialogueBox.text = npc.dialogue[2];
                }
            }
            else if (curResponseTracker == 2 && npc.playerDialogue.Length > 0)
            {
                playerResponse.text = npc.playerDialogue[2];
                if (Input.GetMouseButtonDown(0))
                {
                    SoundManager.PlaySound("3");
                    npcDialogueBox.text = npc.dialogue[3];
                    SceneManager.LoadScene(1);
                }
            }
        }
        else if(distance>2.5f && isTalking == true)
        {
            EndDialogue();
        }
    }


    void StartConversation()
    {
        isTalking = true;
        curResponseTracker = 1;
        dialogueUI.SetActive(true);
        npcName.text = npc.name;
        npcDialogueBox.text = npc.dialogue[0];
        SoundManager.PlaySound("1");


    }

    void EndDialogue()
    {

        isTalking = false;
        dialogueUI.SetActive(false);

    }

 
}
