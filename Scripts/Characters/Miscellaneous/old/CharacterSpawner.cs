// using UnityEngine;
// using FishNet.Object;
// using FishNet.Connection;

// public class CharacterSpawner : NetworkBehaviour
// {
//     [SerializeField] private Transform spawnPoint;
//     [SerializeField] private GameObject emptyCharacterPrefab; // The placeholder prefab
//     [SerializeField] private GameObject[] characterPrefabs;   // The actual character prefabs

//     public override void OnStartServer()
//     {
//         base.OnStartServer();
//         SpawnCharacter();
//     }

//     private void SpawnCharacter()
//     {
//         if (Owner == null)
//         {
//             Debug.LogError("Owner is null in CharacterSpawner.");
//             return;
//         }

//         string selectedCharacter = CharacterManager.GetSelectedCharacter(Owner);
//         GameObject characterPrefab = FindCharacterPrefab(selectedCharacter);

//         if (characterPrefab == null)
//         {
//             Debug.LogError($"Character prefab '{selectedCharacter}' not found. Using default.");
//             characterPrefab = characterPrefabs[0]; // Default fallback
//         }

//         // Spawn the empty character prefab
//         GameObject emptyInstance = Instantiate(emptyCharacterPrefab, spawnPoint.position, spawnPoint.rotation);
//         NetworkObject emptyNetworkObject = emptyInstance.GetComponent<NetworkObject>();
//         Spawn(emptyNetworkObject, Owner);

//         // Replace empty prefab with the selected character
//         ReplaceCharacterPrefab(emptyNetworkObject, characterPrefab);
//     }

//     private void ReplaceCharacterPrefab(NetworkObject emptyNetworkObject, GameObject newCharacterPrefab)
//     {
//         Transform spawnTransform = emptyNetworkObject.transform;
        
//         // Destroy the empty prefab
//         Despawn(emptyNetworkObject);
//         Destroy(emptyNetworkObject.gameObject);

//         // Spawn the selected character in its place
//         GameObject newCharacter = Instantiate(newCharacterPrefab, spawnTransform.position, spawnTransform.rotation);
//         NetworkObject newNetworkObject = newCharacter.GetComponent<NetworkObject>();

//         Spawn(newNetworkObject, Owner);
//         Debug.Log($"Replaced empty prefab with {newCharacterPrefab.name} for player {Owner.ClientId}");
//     }

//     private GameObject FindCharacterPrefab(string characterName)
//     {
//         foreach (GameObject prefab in characterPrefabs)
//         {
//             if (prefab.name == characterName)
//                 return prefab;
//         }
//         return null;
//     }
// }
