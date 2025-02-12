// using FishNet.Object;
// using UnityEngine;

// public class InstantiateWeapon : NetworkBehaviour
// {
//     private CharacterBase characterBase;

//     public override void OnStartClient()
//     {
//         characterBase = GetComponent<CharacterBase>();
//     }

//     public void SpawnWeapon(string weaponName, int playerID, int networkObject)
//     {
//        SpawnWeaponObserversRpc(weaponName, playerID, networkObject);
//     }

//     [ServerRpc]
//     public void SpawnWeaponServerRpc(string weaponName, int playerID)
//     {
//         GameObject weaponPrefab = Resources.Load<GameObject>($"Weapons/{weaponName}");
//         if (weaponPrefab == null)
//         {
//             Debug.LogError($"Weapon prefab {weaponName} not found.");
//             return;
//         }

//         GameObject weaponInstance = Instantiate(weaponPrefab);
//         NetworkObject networkObject = weaponInstance.GetComponent<NetworkObject>();

//         // Assign to the correct player
//         characterBase.weaponInstance = weaponInstance;

//         weaponInstance.transform.SetParent(characterBase.weaponHolder, false);
//         weaponInstance.transform.localPosition = Vector3.zero;
//         weaponInstance.transform.localRotation = Quaternion.identity;

//         WeaponBase weaponBase = weaponInstance.GetComponent<WeaponBase>();
//         weaponBase.SetOwner(characterBase);
//         weaponBase.SaveInitialTransform();

//         // Ensure kinematic property is set
//         Rigidbody rb = weaponInstance.GetComponent<Rigidbody>();
//         if (rb != null)
//             rb.isKinematic = true;

//         base.Spawn(networkObject);
//         networkObject.GiveOwnership(Owner);

//         SpawnWeapon(weaponName, playerID, networkObject.ObjectId);
//         // Sync the weapon across clients
//         //SpawnWeaponObserversRpc();
//     }

//     [ObserversRpc]
//     public void SpawnWeaponObserversRpc(string weaponName, int playerID, int weaponObjectId)
//     {

//         NetworkObject networkObject = FishNet.InstanceFinder.NetworkManager.GetNetworkObject(weaponObjectId);
//         GameObject weaponInstance = networkObject.gameObject;
//         CharacterBase targetPlayer = FindPlayerByID(playerID);

//         if (targetPlayer == null)
//         {
//             Debug.LogError($"Player with ID {playerID} not found!");
//             return;
//         }

//         targetPlayer.weaponInstance = weaponInstance;
//         weaponInstance.transform.SetParent(targetPlayer.weaponHolder, false);
//         weaponInstance.transform.localPosition = Vector3.zero;
//         weaponInstance.transform.localRotation = Quaternion.identity;

//         // Ensure kinematic property is set
//         Rigidbody rb = weaponInstance.GetComponent<Rigidbody>();
//         if (rb != null)
//             rb.isKinematic = true;
//     }

//     private CharacterBase FindPlayerByID(int playerID)
//     {
//         CharacterBase[] allPlayers = FindObjectsOfType<CharacterBase>();
//         foreach (var player in allPlayers)
//         {
//             if (player.PlayerID == playerID)
//                 return player;
//         }
//         return null;
//     }
// }