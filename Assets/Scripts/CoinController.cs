using Unity.Netcode;
using UnityEngine;

public class CoinController : NetworkBehaviour {
    private const string PLAYER = "Player";
  
    [SerializeField] private ScoreManager scoreManager;
    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.tag == PLAYER) {
            DespawnServerRpc();
            if(IsOwner) scoreManager.UpdateScore();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnServerRpc() {
        gameObject.GetComponent<NetworkObject>().Despawn();

    }

}
