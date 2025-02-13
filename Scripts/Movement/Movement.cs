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
    private float acceleration;
    private float maxSpeed;
    private float previousDirection;

    private bool hastAuthority;

    public bool isGrounded;
    private bool isCollidingWithGround; 

    public Movement(Rigidbody rb, SpriteRenderer sprite, LayerMask groundLayer, float speed, float jumpForce, float groundCheckDistance, float acceleration, float maxSpeed)
    {
        this.rb = rb;
        this.sprite = sprite;
        this.groundLayer = groundLayer;
        this.speed = speed;
        this.jumpForce = jumpForce;
        this.groundCheckDistance = groundCheckDistance;
        this.acceleration = acceleration;
        this.maxSpeed = maxSpeed;

        hastAuthority = true;
        
        currentSpeed = 0f;
        previousDirection = 0f;
        
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

    private float jumpBoostX;

    public void HandleJump()
    {
        if ((isGrounded || isCollidingWithGround) && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            jumpBoostX = previousDirection * currentSpeed * 2f;
            HandleAirborneMovement();

        }
    }

    private void HandleAirborneMovement()
    {
        Debug.Log("JUMPBOOST APPLIED");
        rb.velocity = new Vector3(jumpBoostX, rb.velocity.y, rb.velocity.z);
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

            jumpBoostX = 0f;
        }
        else
        {
            isGrounded = false;
            Debug.DrawRay(raycastPosition, Vector3.down * groundCheckDistance, Color.red);
        }

        float sphereRadius = 0.1f;
        isCollidingWithGround = Physics.SphereCast(raycastPosition, sphereRadius, Vector3.down, out RaycastHit sphereHit, groundCheckDistance, groundLayer);
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

        if (x != 0)
        {
            if (Mathf.Sign(x) != Mathf.Sign(previousDirection) && previousDirection != 0)
            {
                currentSpeed *= 0.05f;
            }

            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
            previousDirection = x;
        }
        else
        {
            currentSpeed = 0f;
            previousDirection = 0f;
        }

        float targetSpeed = (isGrounded || isCollidingWithGround) ? currentSpeed : 3f;
        Vector3 moveDirection = new Vector3(x, 0, z) * targetSpeed;
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
        
        if (x > 0)
            sprite.flipX = false;
        else if (x < 0)
            sprite.flipX = true;
    }
}