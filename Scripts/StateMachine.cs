using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private Movement movement;

    [SerializeField] private Animator animator;

    public void UpdateState()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        bool isJumping = !movement.isGrounded;
        bool isMoving = Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f;

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isJumping", isJumping);
    }
}
