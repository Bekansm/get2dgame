using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour {
    public enum Scene {
        Lobby,
        Loading,
        Game,
    }

    [SerializeField] private TextMeshProUGUI createLobbyNameText;
    [SerializeField] private TextMeshProUGUI joinLobbyNameText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private PlayerSpawner playerSpawner;

    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    private async void Start() {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            print("Signed in" + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    private async Task<Allocation> AllocateRelay() {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4 - 1);

            return allocation;
        } catch (RelayServiceException e) {
            Debug.Log(e);

            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation) {
        try {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return relayJoinCode;
        } catch (RelayServiceException e) {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode) {
        try {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        } catch (RelayServiceException e) {
            Debug.Log(e);
            return default;
        }
    }

    // Create lobby with name
    public async void CreateLobby() {
        try {
            string lobbyName = createLobbyNameText.text;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions {
                IsPrivate = false,
                Player = GetPlayerDetails()
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 4, createLobbyOptions);

            Allocation allocation = await AllocateRelay();

            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                     { KEY_RELAY_JOIN_CODE , new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                 }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            NetworkManager.Singleton.StartHost();

            Loader.LoadNetwork(Loader.Scene.GameScene);

            PrintPlayers(lobby);

        } catch (LobbyServiceException e) {
            print(e);
        }
    }

    //Join lobby by name
    public async void JoinLobby() {
        string lobbyId = "";
        string lobbyName = joinLobbyNameText.text;

        try {

            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions {
                Player = GetPlayerDetails()
            };
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            foreach (Lobby lobby in queryResponse.Results) {
                print("Lobby code" + lobby.Name.ToString());

                if (lobby.Name.ToString() == lobbyName) {
                    lobbyId = lobby.Id.ToString();
                }
            }

            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            NetworkManager.Singleton.StartClient();
        } catch (LobbyServiceException e) {
            print(e);

        }
    }


    private Player GetPlayerDetails() {

        return new Player {

            Data = new Dictionary<string, PlayerDataObject> {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerNameText.text) }
                    }
        };
    }

    private void PrintPlayers(Lobby lobby) {
        foreach (Player player in lobby.Players) {
            print(player.Data["PlayerName"].Value);

        }
    }
}
