using UnityEngine;
using FishNet.Object;

public abstract class WeaponBase : NetworkBehaviour
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    private float pickupRadius = 0.4f;
    
    public Rigidbody rb;
    public BoxCollider damageCollider;
    public CapsuleCollider generalCollider;
    public SphereCollider pickupCollider;

    public bool isBroken = false;
    public bool isThrown = false;

    [SerializeField] public CharacterBase owner;
    [SerializeField] public Transform weaponHolder;
    [SerializeField] private PickUpWeapon pickUpWeapon;

    // public void Awake()
    // {
    //     rb = GetComponent<Rigidbody>();
    //     damageCollider = GetComponent<BoxCollider>();
    // }

    protected virtual void Update() {FlipSprite();}

 
    private void FlipSprite()
    {
        if (owner == null || isThrown) return;

        sprite.flipX = owner.sprite.flipX;
    }

#region FishNet Methods

    public override void OnStartClient()
    {
        base.OnStartClient();

        rb = GetComponent<Rigidbody>();
        damageCollider = GetComponent<BoxCollider>();
        generalCollider = GetComponent<CapsuleCollider>();
        pickUpWeapon = GetComponent<PickUpWeapon>();
        
        pickupCollider = gameObject.AddComponent<SphereCollider>();

        pickupCollider.radius = pickupRadius;
        pickupCollider.isTrigger = true;
        pickupCollider.enabled = false;

        damageCollider.enabled = false; 

        weaponHolder = transform.parent;

        if (base.HasAuthority)
        {
            gameObject.tag = "Weapon1";
            pickupCollider.tag = "Weapon1";
        }
        else
        {
            gameObject.tag = "Weapon2";
            pickupCollider.tag = "Weapon2";
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

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (pickUpWeapon == null) return;
    //     pickUpWeapon.PickUp(other);
    // }


    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner && !generalCollider && isThrown) return;

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
    public void DisablePlayerServerRpc(CharacterBase player)
    {
        if (player == null) return;
        player.GetComponent<Respawn>()?.DisablePlayer();
    }
}
