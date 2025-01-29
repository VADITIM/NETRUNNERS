using FishNet.Object;
using UnityEngine;

public class InstantiateWeapon : NetworkBehaviour
{
    private CharacterBase characterBase;

    public override void OnStartClient()
    {
        characterBase = GetComponent<CharacterBase>();
    }

    [ServerRpc]
    public void SpawnWeaponServerRpc()
    {
        GameObject weaponInstance = Instantiate(characterBase.weaponPrefab, characterBase.weaponHolder.position, characterBase.weaponHolder.rotation);
        NetworkObject networkObject = weaponInstance.GetComponent<NetworkObject>();
        
        base.Spawn(networkObject);
        networkObject.GiveOwnership(Owner);
        AttachWeaponObserversRpc(networkObject.ObjectId);
    }

    [ObserversRpc]
    public void AttachWeaponObserversRpc(int weaponObjectId)
    {
        characterBase.weaponInstance.transform.SetParent(characterBase.weaponHolder);
        characterBase.weaponInstance.transform.localPosition = Vector3.zero;
        characterBase.weaponInstance.transform.localRotation = Quaternion.identity;

        WeaponBase weaponScript = characterBase.weaponInstance.GetComponent<WeaponBase>();
        if (weaponScript != null)
        {
            weaponScript.SetOwner(this.characterBase);
        }
    }
}
