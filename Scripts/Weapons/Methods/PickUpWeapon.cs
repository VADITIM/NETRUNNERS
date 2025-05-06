using UnityEngine;
using FishNet.Object;

public class PickUpWeapon : NetworkBehaviour
{
    private WeaponBase weaponBase;
    private bool canPickUp = false; 

    [SerializeField] private Collider playerCollider;

    public void Start()
    {
        weaponBase = GetComponent<WeaponBase>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canPickUp && weaponBase.isThrown)
        {
            PickUp(playerCollider);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player1") && weaponBase.pickupCollider.CompareTag("Weapon1")) ||
            (other.CompareTag("Player2") && weaponBase.pickupCollider.CompareTag("Weapon2")))
        {
            canPickUp = true;
            playerCollider = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == playerCollider)
        {
            canPickUp = false;
            playerCollider = null;
        }
    }

    public ThrowWeapon throwWeapon;

    public void PickUp(Collider other)
    {
        if (weaponBase == null || !weaponBase.isThrown && !throwWeapon.groundCheck) return;

        if (weaponBase.rb == null || weaponBase.weaponHolder == null) return;
        
        canPickUp = false;
        ResetToWeaponHolder();
    }

    private void ResetToWeaponHolder()
    {
        if (weaponBase.weaponHolder == null) return;

        weaponBase.rb.isKinematic = true;
        weaponBase.isThrown = false;
        weaponBase.pickupCollider.enabled = false;

        transform.SetParent(weaponBase.weaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        ResetToWeaponHolderServerRpc();
    }

    [ServerRpc]
    private void ResetToWeaponHolderServerRpc()
    {
        weaponBase.rb.isKinematic = true;
        weaponBase.isThrown = false;
        weaponBase.pickupCollider.enabled = false;

        transform.SetParent(weaponBase.weaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        ResetToWeaponHolderObserversRpc();
    }

    [ObserversRpc]
    private void ResetToWeaponHolderObserversRpc()
    {
        weaponBase.rb.isKinematic = true;
        weaponBase.isThrown = false;
        weaponBase.pickupCollider.enabled = false;

        transform.SetParent(weaponBase.weaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (weaponBase.weaponHolder != null)
        {
            bool isFlipped = weaponBase.weaponHolder.parent.GetComponent<CharacterBase>().sprite.flipX;
            weaponBase.weaponHolder.parent.GetComponent<CharacterBase>().MirrorWeaponHolderPosition(isFlipped);
        }
    }
}
