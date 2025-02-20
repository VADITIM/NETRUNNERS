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

    [SerializeField] private CharacterBase owner;
    [SerializeField] private Throwing throwing;
    [SerializeField] public Transform weaponHolder;
    [SerializeField] private PickUpWeapon pickUpWeapon;

    private float maxMoveDistance = 100f;

    public void Awake()
    {
        weaponCollider = GetComponent<BoxCollider>();   
        pickUpWeapon = gameObject.AddComponent<PickUpWeapon>();
    }

    protected virtual void Update()
    {
        if (throwing == null) return;
        throwing.FixedUpdate();

        HandleMouseMovement();
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
    

#region Mouse Movement

    public void MoveWeaponHolderOnYAxis(float value)
    {
        if (weaponHolder == null || owner == null) return;
        
        float baseY = owner.transform.position.y; 
        float newY = Mathf.Clamp(weaponHolder.position.y + value, baseY - maxMoveDistance, baseY + maxMoveDistance);
        weaponHolder.position = new Vector3(weaponHolder.position.x, newY, weaponHolder.position.z);
    }

    private void HandleMouseMovement()
    {
        if (owner == null) return;

        Vector3 dividePoint = owner.GetDividePoint();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        bool shouldFlip = mouseWorldPos.x < dividePoint.x;
        owner.RequestFlipSpriteServer(shouldFlip); 
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
        pickUpWeapon.PickUp(other);
    }

}
