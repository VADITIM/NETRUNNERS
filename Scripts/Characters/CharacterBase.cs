using UnityEngine;
using FishNet.Object;

public abstract class CharacterBase : NetworkBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    private Rigidbody rb;

    [SerializeField] private float speed = 4.5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 0.18f; // Corrected variable name
    [SerializeField] private float acceleration = 2f;

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
        }
        else
        {
            gameObject.GetComponent<CharacterBase>().enabled = false;
            rb.isKinematic = true;
        }
    }

    protected virtual void Update()
    {
        movement.FixedUpdate();
    }
}