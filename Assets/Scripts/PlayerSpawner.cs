using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;


    private const string GAME_SCENE = "GameScene";
    

    private void Start() {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    public override void OnNetworkSpawn() {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
    }

    //Spawn player on connection
    private void ClientConnected(ulong obj) {
        if (IsHost) {
                GameObject player = Instantiate(playerPrefab);
                  player.GetComponent<NetworkObject>().SpawnAsPlayerObject(obj, true);
    
        
            }
        
    }

    //Spawn host on load
    private void SceneLoaded(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        if (IsHost && sceneName == GAME_SCENE) {
            foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds) {
                GameObject player = Instantiate(playerPrefab);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
     
            }
        }
    }
}
