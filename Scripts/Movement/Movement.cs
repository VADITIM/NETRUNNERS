using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class Movement
{
    private Rigidbody rb;
    private SpriteRenderer sprite;
    private LayerMask groundLayer;

    private float speed;
    private float jumpForce;
    private float groundCheckDistance;

    private float currentSpeed;
    public bool isGrounded;

    public Movement(Rigidbody rb, SpriteRenderer sprite, LayerMask groundLayer, float speed, float jumpForce, float groundCheckDistance)
    {
        this.rb = rb;
        this.sprite = sprite;
        this.groundLayer = groundLayer;

        this.speed = speed;
        this.jumpForce = jumpForce;
        this.groundCheckDistance = groundCheckDistance;

        currentSpeed = speed;
    }

    public void UpdateMovement()
    {
        CheckGroundStatus();
        HandleMovement();
    }

    public void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset Y velocity before jumping
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

        float targetSpeed = isGrounded ? speed : 3f;

        currentSpeed = targetSpeed;

        Vector3 moveDirection = new Vector3(x, 0, z) * currentSpeed;

        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        if (x > 0)
            sprite.flipX = false;
        else if (x < 0)
            sprite.flipX = true;
    }
}
