using UnityEngine;

public class WeaponStateMachine : MonoBehaviour
{
    [SerializeField] private WeaponBase weaponBase;
    [SerializeField] private Animator animator;
    private ThrowWeapon throwWeapon;

    private void Start()
    {
        throwWeapon = GetComponent<ThrowWeapon>();
    }

    private void Update()
    {
        UpdateState();
    }

    public void UpdateState()
    {
        if (weaponBase == null || animator == null || throwWeapon == null) return;

        bool isThrown = weaponBase.isThrown;
        bool isGrounded = throwWeapon.groundCheck;

        animator.SetBool("isThrown", isThrown);
        animator.SetBool("isGrounded", isGrounded);

        if (isThrown && weaponBase.IsOwner)
        {
            HandleThrownState();
        }
        else
        {
            HandleNotThrownState();
        }
    }

    private void HandleThrownState()
    {
        if (animator != null) return;
        animator.SetTrigger("Throw");
    }

    private void HandleNotThrownState()
    {
        if (animator != null) return;
        animator.SetTrigger("Idle");
    }
}