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
    public Abilities abilities; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        instantiateWeapon = GetComponent<InstantiateWeapon>();
        abilities = GetComponent<Abilities>(); 

        if (abilities == null)
        {
            Debug.LogError("Abilities component not found on the character.");
        }
    }

    public override void OnStartClient() 
    {
        base.OnStartClient();
        
        if (base.IsOwner) 
        {
            movement = new Movement(rb, sprite, groundLayer, speed, jumpForce, groundCheckDistance);
            stateMachine = new StateMachine(movement, animator);
            gameObject.tag = "Player1";
            
            string selectedWeapon = WeaponManager.GetSelectedWeapon();
            instantiateWeapon.SpawnWeaponServerRpc(selectedWeapon);
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