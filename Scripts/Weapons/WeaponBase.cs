using UnityEngine;
using FishNet.Object;

public abstract class WeaponBase : NetworkBehaviour
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private float damage;
    [SerializeField] private float pickupRadius = 20f;

    public BoxCollider weaponCollider;
    public Rigidbody rb;
    private SphereCollider pickupTrigger;

    public bool isBroken;
    public bool isAttacking;
    public bool isThrown;

    private CharacterBase owner;
    private Throwing throwing;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public override void OnStartClient()
    {
        base.OnStartClient();

        rb = GetComponent<Rigidbody>();
        weaponCollider = GetComponent<BoxCollider>();
        throwing = GetComponent<Throwing>();

        pickupTrigger = gameObject.AddComponent<SphereCollider>();
        pickupTrigger.enabled = true;
        pickupTrigger.radius = pickupRadius;
        pickupTrigger.isTrigger = true;
        pickupTrigger.enabled = false;

        if (transform.parent != null)
        {
            initialPosition = transform.localPosition;
            initialRotation = transform.localRotation;
        }

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
            MoveBackToInitialPosition();
            Debug.Log("Weapon picked up by player");
        }
    }

    protected virtual void Update()
    {
        if (throwing == null) return;

        throwing.FixedUpdate();
    }

    public void SaveInitialTransform()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    public void SetThrown(bool value)
    {
        isThrown = value;
        if (value)
        {
            transform.SetParent(null);
            pickupTrigger.enabled = true;
        }
    }

    public void SetOwner(CharacterBase owner)
    {
        this.owner = owner;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (isThrown && collision.gameObject == owner.gameObject)
        {
            MoveBackToInitialPosition();
        }
    }

    private void MoveBackToInitialPosition()
    {
        transform.SetParent(owner.weaponHolder, false);
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;
        rb.isKinematic = true;
        isThrown = false;
    }
}