using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject[] characterPrefabs; 

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (Owner == null)
        {
            Debug.LogError("Owner is null in CharacterSpawner.");
            return;
        }

        string selectedCharacter = CharacterManager.GetSelectedCharacter(Owner);
        GameObject characterPrefab = FindCharacterPrefab(selectedCharacter);

        if (characterPrefab == null)
        {
            Debug.LogError($"Character prefab '{selectedCharacter}' not found. Using default.");
            characterPrefab = characterPrefabs[0]; 
        }

        GameObject characterInstance = Instantiate(characterPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkObject networkObject = characterInstance.GetComponent<NetworkObject>();

        Spawn(networkObject, Owner);
    }

    private GameObject FindCharacterPrefab(string characterName)
    {
        foreach (GameObject prefab in characterPrefabs)
        {
            if (prefab.name == characterName)
                return prefab;
        }
        return null;
    }
}
