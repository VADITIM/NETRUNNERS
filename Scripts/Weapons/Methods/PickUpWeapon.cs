using UnityEngine;
using FishNet.Object;

public class PickUpWeapon : NetworkBehaviour
{
    private WeaponBase weaponBase;

    public void Start()
    {
        weaponBase = GetComponent<WeaponBase>();
        if (weaponBase == null)
        {
            Debug.LogError("WeaponBase component not found on " + gameObject.name);
        }
    }

    public void PickUp(Collider other)
    {
        if (weaponBase == null || !weaponBase.isThrown) return;

        if ((other.CompareTag("Player1") && weaponBase.pickupTrigger.CompareTag("Weapon1"))||
            (other.CompareTag("Player2") && weaponBase.pickupTrigger.CompareTag("Weapon2")))
        {
            weaponBase.isThrown = false;
            weaponBase.pickupTrigger.enabled = false;
            ResetToWeaponHolder();
        }
    }

    public void ResetToWeaponHolder()
    {
        if (weaponBase.weaponHolder == null) return;

        weaponBase.rb.isKinematic = true;
        transform.SetParent(weaponBase.weaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        ResetToWeaponHolderServer();
    }

    [ServerRpc]
    public void ResetToWeaponHolderServer()
    {
        if (weaponBase.weaponHolder == null) return;

        weaponBase.rb.isKinematic = true;
        transform.SetParent(weaponBase.weaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        ResetToWeaponHolderClient();
    }

    [ObserversRpc]
    public void ResetToWeaponHolderClient()
    {
        if (weaponBase.weaponHolder == null) return;

        weaponBase.rb.isKinematic = true;
        transform.SetParent(weaponBase.weaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}