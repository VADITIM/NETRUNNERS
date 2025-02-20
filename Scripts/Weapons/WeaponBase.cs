using UnityEngine;
using FishNet.Object;

public abstract class WeaponBase : NetworkBehaviour
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private float damage;
    [SerializeField] private float pickupRadius = 0.4f;
    
    public BoxCollider weaponCollider;
    public Rigidbody rb;
    public SphereCollider pickupTrigger;

    public bool isBroken;
    public bool isAttacking;
    public bool isThrown;

    [SerializeField] public CharacterBase owner;
    [SerializeField] private Throwing throwing;
    [SerializeField] public Transform weaponHolder;
    [SerializeField] private PickUpWeapon pickUpWeapon;
    [SerializeField] private MoveWeapon moveWeapon;
    [SerializeField] Respawn respawn;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        weaponCollider = GetComponent<BoxCollider>();   
        moveWeapon = gameObject.AddComponent<MoveWeapon>();
    }

    protected virtual void Update()
    {
        if (throwing == null) return;
        throwing.FixedUpdate();
    }

#region FishNet Methods

    public override void OnStartClient()
    {
        base.OnStartClient();

        rb = GetComponent<Rigidbody>();
        weaponCollider = GetComponent<BoxCollider>();
        throwing = GetComponent<Throwing>();
        pickUpWeapon = GetComponent<PickUpWeapon>();

        pickupTrigger = gameObject.AddComponent<SphereCollider>();
        pickupTrigger.radius = pickupRadius;
        pickupTrigger.isTrigger = true;
        pickupTrigger.enabled = false;

        weaponHolder = transform.parent;

        if (base.HasAuthority)
        {
            gameObject.tag = "Weapon1";
            pickupTrigger.tag = "Weapon1";
        }
        else
        {
            gameObject.tag = "Weapon2";
            pickupTrigger.tag = "Weapon2";
        }
    }

#endregion

    public void SetThrown(bool value)
    {
        isThrown = value;
        if (value)
        {
            DetachWeaponServerRpc();
        }
    }

    [ServerRpc]
    private void DetachWeaponServerRpc()
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        DetachWeaponClientRpc();
    }

    [ObserversRpc]
    private void DetachWeaponClientRpc()
    {
        transform.SetParent(null);
        rb.isKinematic = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pickUpWeapon == null) return;
        pickUpWeapon.PickUp(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;

        GameObject hitObject = collision.gameObject;
        CharacterBase hitPlayer = hitObject.GetComponent<CharacterBase>();

        if (hitPlayer == null) return;

        if ((gameObject.tag == "Weapon1" && hitObject.CompareTag("Player2")) ||
            (gameObject.tag == "Weapon2" && hitObject.CompareTag("Player1")))
        {
            Debug.Log("Hit player: " + hitPlayer.name);
            DisablePlayerServerRpc(hitPlayer);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisablePlayerServerRpc(CharacterBase player)
    {
        if (player == null) return;
        player.GetComponent<Respawn>()?.DisablePlayer();
    }
}
