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

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        FindObjectOfType<NetworkManager>().onPlayerHeroReceived += LobyManager_onPlayerHeroReceived;

        dialogueMenuComponents = dialogueMenu.GetComponent<DialogueMenu>();

        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            CreateNPCs();
        }
        Invoke(nameof(SpawnPlayerHero), 1);
        

        foreach (Player player in PhotonNetwork.PlayerList) {
            GameTracker.Instance.AddPlayer(player);
        }

    }

    private void LobyManager_onPlayerHeroReceived(PlayerTracker obj) {
        spawnedPlayers.Add(obj.Hero);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        GameTracker.Instance.AddPlayer(newPlayer);

    }    

    public void SpawnPlayerHero() {
        PlayerTracker player = GameTracker.Instance.GetPlayerTracker(PhotonNetwork.LocalPlayer);
        if(player != null) {
            player.Hero = PhotonNetwork.Instantiate(player.PrefabHero.name, playerSpawnLocations[GameTracker.Instance.GetPlayerTrackers().Count-1].position, Quaternion.identity);
            FindObjectOfType<NetworkManager>().SendHero(player.Hero.GetComponent<PhotonView>().ViewID);
            spawnedPlayers.Add(player.Hero);
        }

        if (player.Player.IsMasterClient){
            foreach (GameObject gobject in spawnedNPCs)
            {
                dialogueManagerComponents = gobject.GetComponent<DialogueManager>();
                dialogueManagerComponents.player = player.Hero;
               
            }
        }

        //SEND DATA TO ALL PLAYERS TO BIND HERO
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
            
            dialogueManagerComponents.dialogueUI = dialogueMenu;
            dialogueManagerComponents.npcDialogueBox = dialogueMenuComponents.npcDialogueText;
            dialogueManagerComponents.npcName = dialogueMenuComponents.npcNameText;
            dialogueManagerComponents.playerResponse = dialogueMenuComponents.playerDialogueText;
        }
    }
}
