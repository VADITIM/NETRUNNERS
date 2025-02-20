using UnityEngine;
using FishNet.Object;
using System.Collections;

public abstract class CharacterBase : NetworkBehaviour
{
    public int PlayerID { get; private set; }

    [SerializeField] private Animator animator;
    [SerializeField] public SpriteRenderer sprite;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float speed = 3.5f;
    private float maxSpeed = 8.5f;
    private float acceleration = 20f;
    private float jumpForce = 8f;
    private float jumpBoostX = 10f;
    private float groundCheckDistance = 0.18f;

    public bool isAttacking;
    public float normalAttackFrames = .682f;
    public float specialAttackFrames = .932f;
    private GameObject weaponInstance;

    public GameObject weaponPrefab;
    public Transform weaponHolder;
    private Vector3 weaponHolderPosition;
    private Rigidbody rb;

    private Movement movement;
    public Abilities abilities; 
    private StateMachine stateMachine;
    public WeaponBase weaponBase;

#region FishNet Networking

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if (IsOwner)
        {
            movement = new Movement(rb, sprite, groundLayer, speed, jumpForce, groundCheckDistance, acceleration, maxSpeed);
            stateMachine = new StateMachine(movement, animator, this);
            gameObject.tag = "Player1";
        }
        else
        {
            gameObject.tag = "Player2";
            rb.isKinematic = true;
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        if (weaponHolder == null)
            weaponHolder = transform.Find("WeaponHolder");
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        if (weaponHolder == null)
        {
            GameObject holder = new GameObject("WeaponHolder");
            weaponHolder = holder.transform;
            weaponHolder.SetParent(transform);
            weaponHolder.localPosition = Vector3.zero;
            weaponHolder.localRotation = Quaternion.identity;
        }
    }
#endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        abilities = GetComponent<Abilities>(); 
        weaponHolderPosition = weaponHolder.localPosition;
    }

    protected virtual void Update()
    {
        if (IsOwner && stateMachine != null)
        {
            stateMachine.UpdateState();
            float weaponHolderY = weaponHolder != null ? weaponHolder.position.y : weaponHolderPosition.y;
            
            FlipSprite(rb.velocity.x);
        }

        if (movement == null) return;
        movement.FixedUpdate();
    }

#region Flip Sprites

    public void FlipSprite(float x)
    {
        if (x > 1f) 
        {
            ServerFlipSprite(false); 
        }
        else if (x < -1f) 
        {
            ServerFlipSprite(true);
        }
    }

    [ServerRpc]
    public void ServerFlipSprite(bool flip)
    {
        sprite.flipX = flip;
        if (!weaponBase.isThrown)
        {
            MirrorWeaponHolderPosition(flip);
            InitialWeaponHolderPosition(!flip);
        }
        MoveWeapon moveWeapon = GetComponent<MoveWeapon>();
        if (moveWeapon != null)
        {
            weaponHolder.position = new Vector3(weaponHolder.position.x, moveWeapon.lastPosY, weaponHolder.position.z);
        }

        ObserverFlipSprite(flip);
    }

    [ObserversRpc]
    private void ObserverFlipSprite(bool flip)
    {
        sprite.flipX = flip;
        if (!weaponBase.isThrown)
        {
            MirrorWeaponHolderPosition(flip);
            InitialWeaponHolderPosition(!flip);
        }
        if (weaponHolder == null)
            return;

        MoveWeapon moveWeapon = GetComponent<MoveWeapon>();
        if (moveWeapon != null)
        {
            weaponHolder.position = new Vector3(weaponHolder.position.x, moveWeapon.lastPosY, weaponHolder.position.z);
        }
    }

#region Weapon Holder

    public void MirrorWeaponHolderPosition(bool mirrored)
    {
        if (weaponHolder == null) return;

        Vector3 position = mirrored ? new Vector3(-weaponHolderPosition.x, weaponHolderPosition.y, weaponHolderPosition.z) : weaponHolderPosition;
        weaponHolder.localPosition = position;
    }

    public void InitialWeaponHolderPosition(bool initial)
    {
        if (weaponHolder == null) return;

        Vector3 position = initial ? weaponHolderPosition : new Vector3(-weaponHolderPosition.x, weaponHolderPosition.y, weaponHolderPosition.z);
        weaponHolder.localPosition = position;
    }
#endregion

#endregion

#region States

    public void RequestStateUpdate(bool isMoving, bool isJumping)
    {
        RequestStateUpdateServerRpc(isMoving, isJumping);
    }

    [ServerRpc]
    private void RequestStateUpdateServerRpc(bool isMoving, bool isJumping)
    {
        ApplyStateUpdateObserversRpc(isMoving, isJumping);
    }

    [ObserversRpc]
    private void ApplyStateUpdateObserversRpc(bool isMoving, bool isJumping)
    {
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isJumping", isJumping);
    }

    public void RequestAttackState(AttackType attackType)
    {
        RequestAttackStateServerRpc(attackType);
    }

    [ServerRpc]
    private void RequestAttackStateServerRpc(AttackType attackType)
    {
        ApplyAttackStateObserversRpc(attackType);
    }

    [ObserversRpc]
    private void ApplyAttackStateObserversRpc(AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.NormalGround:
                StartCoroutine(NormalAttackRoutine());
                break;
            case AttackType.SpecialGround:
                StartCoroutine(SpecialAttackRoutine());
                break;
            case AttackType.NormalAir:
                StartCoroutine(NormalAirAttackRoutine());
                break;
            case AttackType.SpecialAir:
                StartCoroutine(SpecialAirAttackRoutine());
                break;
        }
    }
    
#endregion

#region Attack Routines
    private IEnumerator NormalAttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("isNormalAttacking", true);

        yield return new WaitForSeconds(normalAttackFrames);

        isAttacking = false;
        animator.SetBool("isNormalAttacking", false);
    }

    private IEnumerator SpecialAttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("isSpecialAttacking", true);

        yield return new WaitForSeconds(specialAttackFrames);

        isAttacking = false;
        animator.SetBool("isSpecialAttacking", false);
    }

    private IEnumerator NormalAirAttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("isAirNormalAttacking", true);

        yield return new WaitForSeconds(normalAttackFrames);

        isAttacking = false;
        animator.SetBool("isAirNormalAttacking", false);
    }

    private IEnumerator SpecialAirAttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("isAirSpecialAttacking", true);

        yield return new WaitForSeconds(specialAttackFrames);

        isAttacking = false;
        animator.SetBool("isAirSpecialAttacking", false);
    }
#endregion

#region Start Attack Methods
    public void StartNormalAttack()
    {
        isAttacking = true;
        animator.SetBool("isNormalAttacking", true);
    }

    public void StartSpecialAttack()
    {
        isAttacking = true;
        animator.SetBool("isSpecialAttacking", true);
    }

    public void StartAirNormalAttack()
    {
        isAttacking = true;
        animator.SetBool("isAirNormalAttacking", true);
    }

    public void StartAirSpecialAttack()
    {
        isAttacking = true;
        animator.SetBool("isAirSpecialAttacking", true);
    }
#endregion

#region End Attack Methods
    public void EndNormalAttack()
    {
        isAttacking = false;
        animator.SetBool("isNormalAttacking", false);
        animator.Play("idle");
    }

    public void EndSpecialAttack()
    {
        isAttacking = false;
        animator.SetBool("isSpecialAttacking", false);
        animator.Play("idle");
    }

    public void EndAirNormalAttack()
    {
        isAttacking = false;
        animator.SetBool("isAirNormalAttacking", false);
        animator.Play("idle");
    }

    public void EndAirSpecialAttack()
    {
        isAttacking = false;
        animator.SetBool("isAirSpecialAttacking", false);
        animator.Play("idle");
    }
#endregion
}