using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public abstract class CharacterBase : NetworkBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;

    [SerializeField] private float speed = 4.5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 0.18f;
    [SerializeField] private float acceleration = 2f;

    private Rigidbody rb;
    private Movement movement;
    private StateMachine stateMachine;

    public Movement1 movement1;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            movement = new Movement(rb, sprite, groundLayer, speed, jumpForce, groundCheckDistance);
            stateMachine = new StateMachine(movement, animator);
        }
        else 
        {
            gameObject.GetComponent<CharacterBase>().enabled = false;
        }
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();

        movement = new Movement(rb, sprite, groundLayer, speed, jumpForce, groundCheckDistance);
        // movement1 = new Movement1(rb, sprite, groundLayer, speed, jumpForce, groundCheckDistance, acceleration);
        stateMachine = new StateMachine(movement, animator);
    }

    protected virtual void Update()
    {
        movement.UpdateMovement();
        movement.HandleJump();
        stateMachine.UpdateState();
    }
}
