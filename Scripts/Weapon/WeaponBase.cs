using UnityEngine;
using FishNet.Object;

public abstract class WeaponBase : NetworkBehaviour
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private float damage;

    public BoxCollider boxCollider;
    public Rigidbody rb;

    public bool isBroken;
    public bool isAttacking;
    public bool isThrown;

    private CharacterBase owner;
    private Throwing throwing;

    public override void OnStartClient()
    {
        base.OnStartClient();

        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        throwing = GetComponent<Throwing>();

        rb.isKinematic = false;

        if (base.HasAuthority)
        {
            gameObject.tag = "Weapon1";
        }
        else
        {
            gameObject.tag = "Weapon2";
        }
    }

    protected virtual void Update()
    {
        if (throwing == null) return;

        throwing.FixedUpdate();
    }

    public void SetThrown(bool value)
    {
        isThrown = value;
    }

    public void SetOwner(CharacterBase owner)
    {
        this.owner = owner;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (gameObject.CompareTag("Weapon1") &&
            collision.gameObject.CompareTag("Player2"))
        {
            Debug.Log("weapon ONE hit player TWO");
        }

        if (gameObject.CompareTag("Weapon2") &&
            collision.gameObject.CompareTag("Player1"))
        {
            Debug.Log("weapon TWO hit player ONE");
        }
    }
}
