using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private StateMachine stateMachine;
    
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer sprite;
    private Rigidbody rb;

    private float speed = 4.5f;
    private float jumpForce = 5f;
    private float groundCheckDistance = 0.18f;
    private float acceleration = 2f; // Acceleration rate

    private float currentSpeed;
    public bool isGrounded;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        currentSpeed = speed;
    }

    void Update()
    {
        CheckGroundStatus();
        HandleMovement();
        HandleJump();
        stateMachine.UpdateState(); 
    }

    private void CheckGroundStatus()
    {
        RaycastHit raycastHit;
        Vector3 raycastPosition = transform.position;
        raycastPosition.y += 0.1f;

        if (Physics.Raycast(raycastPosition, Vector3.down, out raycastHit, groundCheckDistance, groundLayer))
        {
            isGrounded = true;
            Debug.DrawRay(raycastPosition, Vector3.down * groundCheckDistance, Color.green);

            if (rb.velocity.y < 0)
            {
                Vector3 currentVelocity = rb.velocity;
                currentVelocity.y = 0;
                rb.velocity = currentVelocity;

                Vector3 position = transform.position;
                position.y = raycastHit.point.y + 0.01f; 
                transform.position = position;
            }
        }
        else
        {
            isGrounded = false;
            Debug.DrawRay(raycastPosition, Vector3.down * groundCheckDistance, Color.red);
        }
    }

    private void HandleMovement()
    {
        // float x = Input.GetAxis("Horizontal");
        // float z = Input.GetAxis("Vertical");

        float x = 0f;;
        float z = 0f;;

        if (Input.GetKey(KeyCode.A))
        {
            x = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            x = 1f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            z = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            z = -1f;
        }


        // Adjust target speed based on whether the player is grounded or not
        float targetSpeed = isGrounded ? speed : 3f;

        // Gradually accelerate to the target speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        Vector3 moveDirection = new Vector3(x, 0, z) * currentSpeed;

        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        if (x > 0)
            sprite.flipX = false;
        else if (x < 0)
            sprite.flipX = true;
    }

    public virtual void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset Y velocity before jumping
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
