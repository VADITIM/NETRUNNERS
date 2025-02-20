using UnityEngine;

public class StateMachine
{
    private Movement movement;
    private Animator animator;
    private CharacterBase characterBase;

    public StateMachine(Movement movement, Animator animator, CharacterBase characterBase)
    {
        this.movement = movement;
        this.animator = animator;
        this.characterBase = characterBase;
    }

    public void UpdateState()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isJumping = !movement.isGrounded;
        bool isMoving = Mathf.Abs(x) > 0.1f;

        characterBase.RequestStateUpdate(isMoving, isJumping);

        if (isJumping)
        {
            if (Input.GetKeyDown(KeyCode.P) && !characterBase.isAttacking)
                characterBase.RequestAttackState(AttackType.NormalAir);

            else if (Input.GetKeyDown(KeyCode.O) && !characterBase.isAttacking)
                characterBase.RequestAttackState(AttackType.SpecialAir);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.P) && !characterBase.isAttacking)
                characterBase.RequestAttackState(AttackType.NormalGround);

            else if (Input.GetKeyDown(KeyCode.O) && !characterBase.isAttacking)
                characterBase.RequestAttackState(AttackType.SpecialGround);
        }
    }
}

public enum AttackType
{
    NormalGround,
    SpecialGround,
    NormalAir,
    SpecialAir
}
