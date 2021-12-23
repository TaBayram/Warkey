using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject[] npcPrefab;
    public GameObject dialogueMenu;
    DialogueMenu dialogueMenuComponents;
    DialogueManager dialogueManagerComponents;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    public float exactX;

    public List<GameObject> spawnedPlayers = new List<GameObject>();
    public List<GameObject> spawnedNPCs = new List<GameObject>();
    // Start is called before the first frame update
    private void Start()
    {
        dialogueMenuComponents = dialogueMenu.GetComponent<DialogueMenu>();
        Vector3 randomPositionPlayer = new Vector3(Random.Range(minX, maxX), exactX, Random.Range(minZ, maxZ));

        spawnedPlayers.Add(PhotonNetwork.Instantiate(playerPrefab.name, randomPositionPlayer, Quaternion.identity));
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            for(int i = 0; i < npcPrefab.Length; i++) {
                Vector3 randomPositionNPC = new Vector3(Random.Range(minX, maxX), exactX, Random.Range(minZ, maxZ));
                spawnedNPCs.Add(PhotonNetwork.Instantiate(npcPrefab[i].name, randomPositionNPC, Quaternion.identity));
            }
        }
        foreach (GameObject gobject in spawnedNPCs)
        {
            dialogueManagerComponents = gobject.GetComponent<DialogueManager>();
            dialogueManagerComponents.players = spawnedPlayers;
            dialogueManagerComponents.dialogueUI = dialogueMenu;
            dialogueManagerComponents.npcDialogueBox = dialogueMenuComponents.npcDialogueText;
            dialogueManagerComponents.npcName = dialogueMenuComponents.npcNameText;
            dialogueManagerComponents.playerResponse = dialogueMenuComponents.playerDialogueText;
        }




    }

    // Update is called once per frame
    void Update()
    {

    }
}
