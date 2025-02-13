using UnityEngine;
using FishNet.Object;
using System.Collections;

public abstract class CharacterBase : NetworkBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;

    private float speed = 3.5f;
    private float jumpForce = 5f;
    private float acceleration = 20f;
    private float maxSpeed = 8.5f;
    private float jumpBoostX = 10f;
    [SerializeField] private float groundCheckDistance = 0.18f;

    public int PlayerID { get; private set; }

    public GameObject weaponPrefab;
    public Transform weaponHolder;
    public GameObject weaponInstance;

    private Rigidbody rb;
    private Movement movement;
    public Abilities abilities; 
    
    private StateMachine stateMachine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        abilities = GetComponent<Abilities>(); 
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if (IsOwner)
        {
            movement = new Movement(rb, sprite, groundLayer, speed, jumpForce, groundCheckDistance, acceleration, maxSpeed);
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
    
    [ServerRpc]
    public void DisablePlayerServerRpc()
    {
        DisablePlayerObserversRpc();
    }

    [ObserversRpc]
    private void DisablePlayerObserversRpc()
    {
        gameObject.SetActive(false);
    }
    
    protected virtual void Update()
    {
        if (movement == null) return;

        movement.FixedUpdate();
    }
}