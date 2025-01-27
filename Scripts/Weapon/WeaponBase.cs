using UnityEngine;
using FishNet.Object;

public abstract class WeaponBase : MonoBehaviour
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

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }

    // public override void OnStartClient()
    // {
    //     base.OnStartClient();
    //     boxCollider = GetComponent<BoxCollider>();
    //     rb = GetComponent<Rigidbody>();
        
    //     if (base.HasAuthority)
    //     {
    //        boxCollider = GetComponent<BoxCollider>();
    //     }
    //     else
    //     {
    //         gameObject.GetComponent<WeaponBase>().enabled = false;
    //         rb.isKinematic = true;
    //     }
    // }

    protected virtual void Update()
    {
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider is null.");
        }

        if (isBroken)
        {
            animator.SetBool("isBroken", true);
            return;
        }
        if (isAttacking)
        {
            animator.SetBool("isAttacking", true);
            return;
        }
        if (isThrown)
        {
            animator.SetBool("isThrown", true);
            return;
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Weapon collided with player.");
        }
    }
}