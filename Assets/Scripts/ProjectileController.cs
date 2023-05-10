using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ProjectileController : NetworkBehaviour {
    [SerializeField] private float lifeTime;
    [SerializeField] private float speed;

    //Damage dealt by projectile
    public static float damage { get; private set; } = (10f);

    private void Start() {

        //Destroy projectile after amount of time on server
        if (IsServer) {
            DestroyServerRpc(lifeTime);
        }
    }

    // projectile movement
    private void FixedUpdate() {
        transform.Translate(Vector2.up * Time.fixedDeltaTime * speed);
    }

    private IEnumerator ProjectileLifeTime(float time) {
        yield return new WaitForSeconds(time);
        gameObject.GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc(float time) {
        Destroy(gameObject.GetComponent<NetworkObject>(), time);
        Destroy(gameObject, time);
    }
}
