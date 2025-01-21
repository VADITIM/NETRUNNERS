using UnityEngine;

public class StateMachine
{
    private Movement movement;
    private Animator animator;

    public StateMachine(Movement movement, Animator animator)
    {
        this.movement = movement;
        this.animator = animator;
    }

    public void UpdateState()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isJumping = !movement.isGrounded;
        bool isMoving = Mathf.Abs(x) > 0.1f;

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isJumping", isJumping);
    }
}
