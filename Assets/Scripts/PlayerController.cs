using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour {
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawnPoint;

    //Unassigned
    private PlayerInputActions playerInputActions;
    private Animator animator;
    private ScoreManager scoreManager;

    //Vectors
    private Vector2 inputVector;

    public int score;

 

    //Floats
    [SerializeField] float speed = 2f;
    [SerializeField] float rotationSpeed = 700f;
    private float currentHealth;

    //Strings
    private const string PROJECTILE = "Projectile";
    private const string SPEED = "Speed";


    private void Awake() {
        playerInputActions = new PlayerInputActions();
        animator = GetComponentInChildren<Animator>();
        currentHealth = GetComponentInParent<HealthSystem>().currentHealth;
 

    }

    private void Update() {
        if (IsOwner) {
            Movement();
        }
        if (currentHealth <= 0 && IsServer) {
            gameObject.SetActive(false);
        }

    }

    private void Movement() {
        inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        transform.Translate(inputVector * Time.deltaTime * speed, Space.World);
       
        AnimationHandler(inputVector);
    }

    private void AnimationHandler(Vector2 inputVector) {
        if (inputVector != Vector2.zero) {
            animator.SetFloat(SPEED, 1);
            Quaternion rotateTo = Quaternion.LookRotation(Vector3.forward, inputVector);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateTo, rotationSpeed * Time.deltaTime);
        } else {
            animator.SetFloat(SPEED, 0);
        }
    }

    //Instantiate a projectile on button click
    private void Shoot_performed(InputAction.CallbackContext obj) {
        if (!IsOwner) return;
        SpawnProjectileServerRpc();
    }

    [ServerRpc]
    private void SpawnProjectileServerRpc() {
        GameObject projectileObject = Instantiate(projectile, projectileSpawnPoint.transform.position, projectileSpawnPoint.transform.rotation);
        projectileObject.GetComponent<NetworkObject>().Spawn();
    }


    //Adjust health on collision with projectile
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == PROJECTILE) {
            gameObject.GetComponentInParent<HealthSystem>().DamageTakenClientRpc(ProjectileController.damage);

            if (IsServer) collision.gameObject.GetComponent<ProjectileController>().DestroyServerRpc(0);
        }
    }

    private void OnEnable() {
        playerInputActions.Enable();
        playerInputActions.Player.Shoot.performed += Shoot_performed;
    }

    private void OnDisable() {
        playerInputActions.Disable();
        playerInputActions.Player.Shoot.performed -= Shoot_performed;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc() {
        Destroy(gameObject.GetComponent<NetworkObject>());
        Destroy(gameObject);
    }
}
