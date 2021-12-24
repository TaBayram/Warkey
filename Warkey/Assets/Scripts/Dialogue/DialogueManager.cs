using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public NPC npc;

    bool isTalking = false;

    float distance;
    int currentPlayerMessageIndex = 0;

    int currentNPCMessageIndex = 0;

    public AudioSource audioSource;

    public List<GameObject> players;
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
        foreach (GameObject player in players)
        {
            distance = Vector3.Distance(player.transform.position, this.transform.position);
            if (distance <= 2.5f)
            {
                if (Input.GetKeyDown(KeyCode.E) && isTalking == false)
                {
                    StartConversation();
                }
                else if (Input.GetKeyDown(KeyCode.E) && isTalking == true)
                {
                    EndDialogue();
                }
                bool hasIndexChanged = false;
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {

                    currentPlayerMessageIndex++; hasIndexChanged = true;
                    if (currentPlayerMessageIndex >= npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes.Length - 1)
                    {
                        currentPlayerMessageIndex = npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes.Length - 1;
                    }
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    currentPlayerMessageIndex--; hasIndexChanged = true;
                    if (currentPlayerMessageIndex < 0)
                    {
                        currentPlayerMessageIndex = 0;
                    }
                }
                //trigger dialogue


                //for (int i = 0; i < curResponseTracker.Length; i++)
                //{

                //}
                if (hasIndexChanged)
                {
                    int playerMessageIndex = npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes[currentPlayerMessageIndex];
                    playerResponse.text = npc.playerDialogMessages[playerMessageIndex].dialogMessage.message;


                }
                if (Input.GetMouseButtonDown(0))
                {
                    int playerMessageIndex = npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes[currentPlayerMessageIndex];
                    ShowNPCMessage(npc.playerDialogMessages[playerMessageIndex].npcDialogIndex);
                    if (npc.nPCDialogMessages[currentNPCMessageIndex].isExitMessage)
                    {
                        EndDialogue();
                    }
                    if (npc.nPCDialogMessages[currentNPCMessageIndex].isQuestAccepter)
                    {
                        SceneManager.LoadScene("Terrain Scene");
                    }
                }

            }
            else if (distance > 2.5f && isTalking == true)
            {
                EndDialogue();
            }
        }
    }


    void StartConversation()
    {
        isTalking = true;
        currentPlayerMessageIndex = 1;
        dialogueUI.SetActive(true);
        npcName.text = npc.name;
        ShowNPCMessage(0);


    }

    void EndDialogue()
    {

        isTalking = false;
        dialogueUI.SetActive(false);

    }
    private void ShowNPCMessage(int index)
    {
        currentNPCMessageIndex = index;
        npcDialogueBox.text = npc.nPCDialogMessages[index].dialogMessage.message;
        audioSource.clip = npc.nPCDialogMessages[index].dialogMessage.audio;
        audioSource.Play();
        currentPlayerMessageIndex = 0;
        int playerMessageIndex = npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes[currentPlayerMessageIndex];
        playerResponse.text = npc.playerDialogMessages[playerMessageIndex].dialogMessage.message;


    }


}
