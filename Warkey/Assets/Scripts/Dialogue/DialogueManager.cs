using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

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

    [SerializeField] Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit raycastHit, 100f, 1 << LayerMask.NameToLayer("Ground"))) {
            transform.position = raycastHit.point;
        }

    }

    private void Update() {
        if (!audioSource.isPlaying) {
            animator.SetInteger("talkState", 0);
        }
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
                    if(npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes != null && npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes.Length > 0) {
                        int playerMessageIndex = npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes[currentPlayerMessageIndex];
                        playerResponse.text = npc.playerDialogMessages[playerMessageIndex].dialogMessage.message;
                    }

                }
                if (Input.GetMouseButtonDown(0))
                {
                    int playerMessageIndex = npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes[currentPlayerMessageIndex];
                    ShowNPCMessage(npc.playerDialogMessages[playerMessageIndex].npcDialogIndex);
                    if (npc.nPCDialogMessages[currentNPCMessageIndex].isExitMessage) {
                        EndDialogue();
                        return;
                    }
                    if (npc.nPCDialogMessages[currentNPCMessageIndex].isQuestAccepter) {
                        StartQuest();
                        return;
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
        if (npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes != null && npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes.Length > 0) {
            int playerMessageIndex = npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes[currentPlayerMessageIndex];
            playerResponse.text = npc.playerDialogMessages[playerMessageIndex].dialogMessage.message;
        }

        animator.SetInteger("talkState", 1);
    }


    private void StartQuest() {
        GameTracker.Instance.WorldSettingsHolder.SetWorldSettings();
        GameTracker.Instance.WorldSettingsHolder.SendWorldSettings();


        Invoke(nameof(ChangeScene), 2);
        
    }

    private void ChangeScene() {
        SceneManager.LoadScene("WorldScene");
    }



}
