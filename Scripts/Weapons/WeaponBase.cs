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

    private CharacterBase owner;
    private Throwing throwing;
    private Transform weaponHolder;

    public override void OnStartClient()
    {
        base.OnStartClient();

        rb = GetComponent<Rigidbody>();
        weaponCollider = GetComponent<BoxCollider>();
        throwing = GetComponent<Throwing>();

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

    private void OnTriggerEnter(Collider other)
    {
        if (!isThrown) return;

        if ((other.CompareTag("Player1") && pickupTrigger.CompareTag("Weapon1")) ||
            (other.CompareTag("Player2") && pickupTrigger.CompareTag("Weapon2")))
        {
            isThrown = false;
            pickupTrigger.enabled = false;
            ResetToWeaponHolder();
            PickupWeaponServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickupWeaponServerRpc()
    {
        PickupWeaponObserversRpc();
    }

    [ObserversRpc]
    private void PickupWeaponObserversRpc()
    {
        ResetToWeaponHolder();
    }

    protected virtual void Update()
    {
        if (throwing == null) return;
        throwing.FixedUpdate();
    }

    public void SetThrown(bool value)
    {
        isThrown = value;
        if (value)
        {
            transform.SetParent(null);
            pickupTrigger.enabled = true;
            rb.isKinematic = false;
        }
    }

    public void SetOwner(CharacterBase newOwner)
    {
        owner = newOwner;
        if (owner != null)
        {
            weaponHolder = owner.transform;
            ResetToWeaponHolder();
        }
    }

    private void ResetToWeaponHolder()
    {
        if (weaponHolder == null) return;

        transform.SetParent(weaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        rb.isKinematic = true;
    }
}