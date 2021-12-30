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

    [HideInInspector] public GameObject player;
    [HideInInspector] public GameObject dialogueUI;

    [HideInInspector] public Text npcName;
    [HideInInspector] public Text npcDialogueBox;
    [HideInInspector] public Text playerResponse;
    [HideInInspector] public Text playerResponseUpper;
    [HideInInspector] public Text playerResponseLower;

    public GameObject InteractWithE;

    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit raycastHit, 100f, 1 << LayerMask.NameToLayer("Ground"))) {
            transform.position = raycastHit.point;
        }

    }

    private void Update() {
        if (player == null) return;
        if (!PhotonNetwork.IsMasterClient) return;

        if (!audioSource.isPlaying) {
            animator.SetInteger("talkState", 0);
        }
        
        distance = (player.transform.position- this.transform.position).magnitude;
        if (distance <= 2.5f)
        {
            InteractWithE.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E) && isTalking == false)
            {
                StartConversation();
            }
            else if (Input.GetKeyDown(KeyCode.E) && isTalking == true)
            {
                EndDialogue();
            }
            if (!isTalking) return;

            bool hasIndexChanged = false;
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {

                currentPlayerMessageIndex++; hasIndexChanged = true;
                if (currentPlayerMessageIndex >= npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes.Length - 1)
                {
                    currentPlayerMessageIndex = npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes.Length - 1;
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
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
                UpdatePlayerResponse();

            }
            if (Input.GetMouseButtonDown(0))
            {
                int playerMessageIndex = npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes[currentPlayerMessageIndex];
                ShowNPCMessage(npc.playerDialogMessages[playerMessageIndex].npcDialogIndex);
                if (npc.nPCDialogMessages[currentNPCMessageIndex].isExitMessage)
                {
                    EndDialogue();
                    return;
                }
                if (npc.nPCDialogMessages[currentNPCMessageIndex].isQuestAccepter)
                {
                    StartQuest();
                    return;
                }
            }

        }
        else if (distance > 2.5f && isTalking == true){
            EndDialogue();
            InteractWithE.SetActive(false);

        }
        else {
            InteractWithE.SetActive(false);

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
        UpdatePlayerResponse();

        animator.SetInteger("talkState", 1);
    }


    private void StartQuest() {
        EndDialogue();
        GameTracker.Instance.WorldSettingsHolder.SetWorldSettings();
        GameTracker.Instance.WorldSettingsHolder.SendWorldSettings();

        Invoke(nameof(ChangeScene), 2);
    }

    private void ChangeScene() {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        LoadScene.SceneIndex = LoadScene.Scenes.World;
        SceneManager.LoadScene((int)LoadScene.Scenes.World);
    }

    private void UpdatePlayerResponse() {

        if (npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes != null && npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes.Length > 0) {
            int playerMessageIndex = npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes[currentPlayerMessageIndex];
            playerResponse.text = npc.playerDialogMessages[playerMessageIndex].dialogMessage.message;
        

            if (currentPlayerMessageIndex - 1 >= 0) {
                playerMessageIndex = npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes[currentPlayerMessageIndex - 1];
                playerResponseUpper.text = npc.playerDialogMessages[playerMessageIndex].dialogMessage.message;
            }
            else {
                playerResponseUpper.text = "";
            }


            if (currentPlayerMessageIndex + 1 < npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes.Length) {
                playerMessageIndex = npc.nPCDialogMessages[currentNPCMessageIndex].playerDialogIndexes[currentPlayerMessageIndex + 1];
                playerResponseLower.text = npc.playerDialogMessages[playerMessageIndex].dialogMessage.message;
            }
            else {
                playerResponseLower.text = "";
            }

        }

    }


}
