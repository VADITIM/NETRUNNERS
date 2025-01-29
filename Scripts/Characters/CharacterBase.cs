using UnityEngine;
using FishNet.Object;

public abstract class CharacterBase : NetworkBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;

    [SerializeField] private float speed = 4.5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 0.18f;
    [SerializeField] private float acceleration = 2f;

    [SerializeField] public GameObject weaponPrefab;
    [SerializeField] public Transform weaponHolder;
    public GameObject weaponInstance;

    private Rigidbody rb;

    private Movement movement;
    private StateMachine stateMachine;
    private InstantiateWeapon instantiateWeapon;

    public override void OnStartClient()
    {
        base.OnStartClient();

        rb = GetComponent<Rigidbody>();
        instantiateWeapon = GetComponent<InstantiateWeapon>();

        if (base.HasAuthority)
        {
            movement = new Movement(rb, sprite, groundLayer, speed, jumpForce, groundCheckDistance);
            stateMachine = new StateMachine(movement, animator);
            gameObject.tag = "Player1";

            instantiateWeapon.SpawnWeaponServerRpc();
        }
        else
        {
            gameObject.GetComponent<CharacterBase>().enabled = false;
            rb.isKinematic = true;
            gameObject.tag = "Player2";
        }
    }

    protected virtual void Update()
    {
        if (movement == null) return;

        movement.FixedUpdate();
    }
}
