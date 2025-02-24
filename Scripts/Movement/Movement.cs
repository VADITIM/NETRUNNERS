using UnityEngine;
using System.Collections;

public class Movement
{
    private Rigidbody rb;
    private SpriteRenderer sprite;
    private LayerMask groundLayer;
    private CharacterBase characterBase;
    
    private bool hastAuthority;

    private float speed;
    public float currentSpeed;
    private float maxSpeed; 
    private bool isMaxSpeed;
    private float acceleration;

    private float jumpForce;
    private float gravity;
    public float groundCheckDistance;
    
    private float previousDirection;


    public bool isGrounded;
    private bool isCollidingWithGround; 

    public float jumpBoostX;
    private bool jumpBoostReady;

    private float cooldown = 1f;
    private float cooldownTimer = 0f;
    private bool timerActive = false;

    private MonoBehaviour coroutineHandler;

    public Movement(Rigidbody rb, SpriteRenderer sprite, LayerMask groundLayer, float gravity, float speed, float currentSpeed, float jumpBoostX, float jumpForce, float groundCheckDistance, float acceleration, float maxSpeed, MonoBehaviour coroutineHandler)
    {
        this.rb = rb;
        this.sprite = sprite;
        this.groundLayer = groundLayer;
        this.speed = speed;
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
        this.currentSpeed = currentSpeed;
        this.jumpForce = jumpForce;
        this.jumpBoostX = jumpBoostX;
        this.gravity = gravity;
        this.groundCheckDistance = groundCheckDistance;
        this.coroutineHandler = coroutineHandler;

        hastAuthority = true;
        
        currentSpeed = 0f;
        previousDirection = 0f;
        
        GameObject playerObject = rb.gameObject;
        Collider playerCollider = playerObject.GetComponent<Collider>();
        characterBase = rb.GetComponent<CharacterBase>();

        if (playerCollider == null) return;

        PhysicMaterial physicsMaterial = new PhysicMaterial
        {
            dynamicFriction = 0.6f,
            staticFriction = 0.6f,
            frictionCombine = PhysicMaterialCombine.Maximum,
            bounceCombine = PhysicMaterialCombine.Minimum
        };
        playerCollider.material = physicsMaterial;
    }

    public void FixedUpdate()
    {
        if (!hastAuthority) return;
       
        CheckGroundStatus();
        HandleMovement(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // CheckVelocity();
        HandleJump();
        ApplyGravity();
    }


    private Coroutine jumpBoostCoroutine;

    // private void CheckVelocity()
    // {
    //     if (currentSpeed >= maxSpeed)
    //     {
    //         if (!isMaxSpeed)
    //         {
    //             isMaxSpeed = true;
    //             Debug.Log("Max Speed Reached");

    //             if (jumpBoostCoroutine == null)
    //             {
    //                 jumpBoostCoroutine = coroutineHandler.StartCoroutine(JumpBoostCooldown());
    //             }
    //         }
    //     }
    //     else
    //     {
    //         isMaxSpeed = false;
    //         jumpBoostReady = false;

    //         if (jumpBoostCoroutine != null)
    //         {
    //             coroutineHandler.StopCoroutine(jumpBoostCoroutine);
    //             jumpBoostCoroutine = null;
    //         }
    //     }
    // }

    // private IEnumerator JumpBoostCooldown()
    // {
    //     float elapsedTime = 0f;
    //     Debug.Log("Timer started");

    //     while (elapsedTime < cooldown)
    //     {
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }

    //     jumpBoostReady = true;
    //     jumpBoostCoroutine = null;
    //     Debug.Log("Timer ended JumpBoostReady");
    // }
    
    private void HandleMovement(float x, float z)
    {
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
            Accelerate(x);
        }
        else
        {
            if (isGrounded || isCollidingWithGround)
            {
                currentSpeed = 0f;
                previousDirection = 0f;
            }
        }

        HandleAirborneMovement(x, z);
    }

    private void HandleAirborneMovement(float x, float z)
    {
        float targetSpeed = (isGrounded || isCollidingWithGround) ? currentSpeed : currentSpeed * 0.98f; 
        Vector3 moveDirection = new Vector3(x != 0 ? x : previousDirection, 0, z) * targetSpeed;

        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
    }

    public void HandleJump()
    {
        if ((isGrounded || isCollidingWithGround) && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            isMaxSpeed = false;
        }
        // else if (jumpBoostReady && Input.GetButton("Jump"))
        // {
        //     rb.velocity = new Vector3(rb.velocity.x * jumpBoostX, jumpForce, rb.velocity.z);
        //     jumpBoostReady = false;
        //     Debug.Log("Jump Boost Used");
        // }
    }

    private void Accelerate(float x)
    {
        if (Mathf.Sign(x) != Mathf.Sign(previousDirection) && previousDirection != 0)
        {
            if (isGrounded) currentSpeed *= 0.02f; 
            else currentSpeed *= 0.6f;
        }
        
        if (!isGrounded)
        {
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed * .7f);
        }
        else 
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        }
        previousDirection = x;
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
        isCollidingWithGround = Physics.SphereCast(raycastPosition, sphereRadius, Vector3.down, out RaycastHit sphereHit, groundCheckDistance, groundLayer);
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }
    }
}