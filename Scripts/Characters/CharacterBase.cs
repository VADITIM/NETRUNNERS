using UnityEngine;
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

    private float gravity = 9.81f;
    private Vector3 velocity;
    public bool isGrounded;
    private CharacterController controller;


    public override void OnStartClient()
    {
        base.OnStartClient();
        controller = GetComponent<CharacterController>();

        if (base.HasAuthority)
        {
            
        }
        else 
        {
            controller.enabled = false;
        }
    }

    protected virtual void Update()
    {
        GroundCheck();
        Move();
        Jump();
    }

    protected virtual void GroundCheck()
    {
        Vector3 start = transform.position;
        Vector3 direction = Vector3.down;
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    protected virtual void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 direction = new Vector3(horizontal, 0, 0);
        controller.Move(direction * speed * Time.deltaTime);
    }

    protected virtual void Jump()
    {
        velocity.y += -gravity * Time.deltaTime;

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce;
        }

        controller.Move(velocity * Time.deltaTime);
    }
}