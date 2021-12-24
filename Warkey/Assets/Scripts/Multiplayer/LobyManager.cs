using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobyManager : MonoBehaviourPunCallbacks
{
    public GameObject[] npcPrefabs;
    public GameObject dialogueMenu;
    DialogueMenu dialogueMenuComponents;
    DialogueManager dialogueManagerComponents;

    [SerializeField] private List<Transform> playerSpawnLocations;
    [SerializeField] private List<Transform> npcSpawnLocations;

    [HideInInspector] public List<GameObject> spawnedPlayers = new List<GameObject>();    
    [HideInInspector] public List<GameObject> spawnedNPCs = new List<GameObject>();
    // Start is called before the first frame update
    private void Start()
    {
        dialogueMenuComponents = dialogueMenu.GetComponent<DialogueMenu>();

        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            CreateNPCs();
        }

        var player = new PlayerTracker();
        player.IsMine = true;
        Debug.Log("S "+PhotonNetwork.LocalPlayer.NickName);
        GameTracker.Instance.PlayerTrackers.Add(player);
        player.Hero = PhotonNetwork.Instantiate(player.PrefabHero.name, playerSpawnLocations[spawnedPlayers.Count].position, Quaternion.identity);
        spawnedPlayers.Add(player.Hero);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log("Entered " + newPlayer.UserId);
        foreach (GameObject gobject in spawnedNPCs) {
            dialogueManagerComponents = gobject.GetComponent<DialogueManager>();
            dialogueManagerComponents.players = spawnedPlayers;
        }
    }

    public void CreateNPCs() {
        int npcSpawnAmount = Random.Range(2, 4);
        List<Transform> availableLocations = new List<Transform>(npcSpawnLocations);

        for (int i = 0; i < npcSpawnAmount; i++) {
            int randomIndex = Random.Range(0, availableLocations.Count);
            Vector3 position = availableLocations[randomIndex].position;
            availableLocations.RemoveAt(randomIndex);
            randomIndex = Random.Range(0, npcPrefabs.Length);
            spawnedNPCs.Add(PhotonNetwork.Instantiate(npcPrefabs[randomIndex].name, position, Quaternion.identity));
        }
        
        foreach (GameObject gobject in spawnedNPCs) {
            dialogueManagerComponents = gobject.GetComponent<DialogueManager>();
            dialogueManagerComponents.players = spawnedPlayers;
            dialogueManagerComponents.dialogueUI = dialogueMenu;
            dialogueManagerComponents.npcDialogueBox = dialogueMenuComponents.npcDialogueText;
            dialogueManagerComponents.npcName = dialogueMenuComponents.npcNameText;
            dialogueManagerComponents.playerResponse = dialogueMenuComponents.playerDialogueText;
        }
    }
}
