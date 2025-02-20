using UnityEngine;
using FishNet.Object;

public class PickUpWeapon : NetworkBehaviour
{
    private WeaponBase weaponBase;

    public void Start()
    {
        weaponBase = GetComponent<WeaponBase>();
    }

    public void PickUp(Collider other)
    {
        if (weaponBase == null || !weaponBase.isThrown) return;

        if ((other.CompareTag("Player1") && weaponBase.pickupTrigger.CompareTag("Weapon1")) ||
            (other.CompareTag("Player2") && weaponBase.pickupTrigger.CompareTag("Weapon2")))
        {
            if (weaponBase.rb == null || weaponBase.weaponHolder == null) return;

            ResetToWeaponHolder();
        }
    }

    private void ResetToWeaponHolder()
    {
        if (weaponBase.weaponHolder == null) return;

        weaponBase.rb.isKinematic = true;
        weaponBase.isThrown = false;
        weaponBase.pickupTrigger.enabled = false;

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
        weaponBase.pickupTrigger.enabled = false;

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
        weaponBase.pickupTrigger.enabled = false;

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
