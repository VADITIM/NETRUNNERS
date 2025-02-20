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

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        weaponCollider = GetComponent<BoxCollider>();   
        pickUpWeapon = gameObject.AddComponent<PickUpWeapon>();
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
            transform.SetParent(null);
            pickupTrigger.enabled = true;
            rb.isKinematic = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pickUpWeapon == null) return;
        pickUpWeapon.PickUp(other);
    }

}
