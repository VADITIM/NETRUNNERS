using UnityEngine;

public class StateMachine
{
    private Animator animator;
    private CharacterBase charBase;

    public StateMachine(CharacterBase charBase, Animator animator)
    {
        this.charBase = charBase;
        this.animator = animator;
    }

    public void UpdateState()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isJumping = !charBase.isGrounded;
        bool isMoving = Mathf.Abs(x) > 0.1f;

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isJumping", isJumping);
    }
}
