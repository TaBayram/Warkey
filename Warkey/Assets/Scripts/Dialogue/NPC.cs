using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NPC file", menuName = "NPC Files Archive")]
public class NPC : ScriptableObject
{

    //public new string name;
    //[TextArea(3, 15)]
    //public string[] dialogue;
    //[TextArea(3, 15)]
    //public string[] playerDialogue;

    public NPCDialogMessage[] nPCDialogMessages;
    public PlayerDialogMessage[] playerDialogMessages;



    [System.Serializable]
    public struct DialogMessage
    {
        public string message;
        public AudioClip audio;
    }

    [System.Serializable]
    public struct NPCDialogMessage
    {
        public DialogMessage dialogMessage;
        public int[] playerDialogIndexes;
        public bool isExitMessage;
        public bool isQuestAccepter;
    }

    [System.Serializable]
    public struct PlayerDialogMessage
    {
        public DialogMessage dialogMessage;
        public int npcDialogIndex;
   
    }
}
