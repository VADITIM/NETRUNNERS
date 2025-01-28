using UnityEngine;
using FishNet.Object;
using FishNet.Managing;

public abstract class CharacterBase : NetworkBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private Transform weaponHolder;

    [SerializeField] private float speed = 4.5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 0.18f;
    [SerializeField] private float acceleration = 2f;

    private Rigidbody rb;
    private GameObject weaponInstance;

    private Movement movement;
    private StateMachine stateMachine;

    public override void OnStartClient()
    {
        base.OnStartClient();
        rb = GetComponent<Rigidbody>();

        if (base.HasAuthority)
        {
            movement = new Movement(rb, sprite, groundLayer, speed, jumpForce, groundCheckDistance);
            stateMachine = new StateMachine(movement, animator);
            gameObject.tag = "Player1";

            if (weaponPrefab != null)
            {
                // Request the server to spawn and attach the weapon
                SpawnWeaponServerRpc();
            }
        }
        else
        {
            gameObject.GetComponent<CharacterBase>().enabled = false;
            rb.isKinematic = true;
            gameObject.tag = "Player2";
        }
    }

    protected virtual void Update()
    {
        if (movement != null)
        {
            movement.FixedUpdate();
        }
    }

    [ServerRpc]
    private void SpawnWeaponServerRpc()
    {
        // Spawn the weapon on the server (using NetworkObject.Spawn)
        GameObject weaponInstance = Instantiate(weaponPrefab, weaponHolder.position, weaponHolder.rotation);

        // Ensure the object has a NetworkObject component and spawn it across the network
        NetworkObject networkObject = weaponInstance.GetComponent<NetworkObject>();
        base.Spawn(networkObject);

        // After spawning, notify all clients to attach the weapon to the correct position
        AttachWeaponObserversRpc(networkObject.ObjectId);
    }

    [ObserversRpc]
    private void AttachWeaponObserversRpc(int weaponObjectId)
    {
        weaponInstance.transform.SetParent(weaponHolder);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity;

        // Set up the weapon script (e.g., weapon ownership)
        WeaponBase weaponScript = weaponInstance.GetComponent<WeaponBase>();
        if (weaponScript != null)
        {
            weaponScript.SetOwner(this);
        }
    }
}
