using UnityEngine;
using FishNet.Object;

public abstract class WeaponBase : NetworkBehaviour
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private float damage;

    public BoxCollider boxCollider;
    private Rigidbody rb;

    private bool isBroken;
    private bool isAttacking;
    private bool isThrown;

    private CharacterBase owner;

    public void SetOwner(CharacterBase owner)
    {
        this.owner = owner;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        
        if (base.HasAuthority)
        {
           boxCollider = GetComponent<BoxCollider>();
           gameObject.tag = "Weapon1";
           gameObject.SetActive(true);
        }
        else
        {
           gameObject.tag = "Weapon2";
           rb.isKinematic = true;
        }
    }

    protected virtual void Update()
    {
        Throw();

        // animator.SetBool("isBroken", true);
        // animator.SetBool("isAttacking", true);
        // animator.SetBool("isThrown", true);
    }


    protected virtual void Throw()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Throwing weapon.");
            isThrown = true;
            rb.isKinematic = false;
            rb.AddForce(transform.right * 0.4f , ForceMode.Impulse);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (gameObject.CompareTag("Weapon1") && collision.gameObject.CompareTag("Player2"))
        {
            Debug.Log("weapon ONE hit player TWO");
        }
        else if (gameObject.CompareTag("Weapon2") && collision.gameObject.CompareTag("Player1"))
        {
            Debug.Log("weapon TWO hit player ONE");
        }
    }
}