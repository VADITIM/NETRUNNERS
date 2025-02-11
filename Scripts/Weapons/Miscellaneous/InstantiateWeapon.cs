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
    public void SpawnWeaponServerRpc(string weaponName)
    {
        GameObject weaponPrefab = Resources.Load<GameObject>($"Weapons/{weaponName}");
        GameObject weaponInstance = Instantiate(weaponPrefab);
        NetworkObject networkObject = weaponInstance.GetComponent<NetworkObject>();

        characterBase.weaponInstance = weaponInstance;

        // Parent to the weapon holder before spawning
        weaponInstance.transform.SetParent(characterBase.weaponHolder, false);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity;

        // Save the initial position and rotation
        WeaponBase weaponBase = weaponInstance.GetComponent<WeaponBase>();
        weaponBase.SaveInitialTransform();

        base.Spawn(networkObject);
        networkObject.GiveOwnership(Owner);
    }
}