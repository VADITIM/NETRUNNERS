using UnityEngine;

public class Movement
{
    private Rigidbody rb;
    private SpriteRenderer sprite;
    private LayerMask groundLayer;
    
    private float speed;
    private float jumpForce;
    public float groundCheckDistance;
    private float currentSpeed;
    public bool isGrounded;
    private bool hastAuthority;
    private bool isCollidingWithGround; 

    public Movement(Rigidbody rb, SpriteRenderer sprite, LayerMask groundLayer, float speed, float jumpForce, float groundCheckDistance)
    {
        this.rb = rb;
        this.sprite = sprite;
        this.groundLayer = groundLayer;
        this.speed = speed;
        this.jumpForce = jumpForce;
        this.groundCheckDistance = groundCheckDistance;
        currentSpeed = speed;
        hastAuthority = true;
        
        GameObject playerObject = rb.gameObject;
        Collider playerCollider = playerObject.GetComponent<Collider>();
        if (playerCollider != null)
        {
            PhysicMaterial physicsMaterial = new PhysicMaterial
            {
                dynamicFriction = 0.6f,
                staticFriction = 0.6f,
                frictionCombine = PhysicMaterialCombine.Maximum,
                bounceCombine = PhysicMaterialCombine.Minimum
            };
            playerCollider.material = physicsMaterial;
        }
    }

    public void FixedUpdate()
    {
        if (!hastAuthority) return;
       
        CheckGroundStatus();
        HandleMovement();
        HandleJump();
    }

    public void HandleJump()
    {
        if ((isGrounded || isCollidingWithGround) && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void CheckGroundStatus()
    {
        RaycastHit raycastHit;
        Vector3 raycastPosition = rb.transform.position;
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
                Vector3 position = rb.transform.position;
                position.y = raycastHit.point.y + 0.01f;
                rb.transform.position = position;
            }
        }
        else
        {
            isGrounded = false;
            Debug.DrawRay(raycastPosition, Vector3.down * groundCheckDistance, Color.red);
        }

        float sphereRadius = 0.1f;
        isCollidingWithGround = Physics.SphereCast(
            raycastPosition,
            sphereRadius,
            Vector3.down,
            out RaycastHit sphereHit,
            groundCheckDistance,
            groundLayer
        );
    }

    private void HandleMovement()
    {
        float x = 0f;
        float z = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            x = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            x = 1f;
        }
        
        float targetSpeed = (isGrounded || isCollidingWithGround) ? speed : 3f;
        currentSpeed = targetSpeed;
        Vector3 moveDirection = new Vector3(x, 0, z) * currentSpeed;
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
        
        if (x > 0)
            sprite.flipX = false;
        else if (x < 0)
            sprite.flipX = true;
    }
}