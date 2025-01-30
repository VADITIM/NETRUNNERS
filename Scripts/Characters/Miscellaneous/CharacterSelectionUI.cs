using UnityEngine;
using UnityEngine.UI;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;

public class CharacterSelectionUI : NetworkBehaviour
{
    [SerializeField] private Button[] characterButtons;
    [SerializeField] private string[] characterNames;
    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();

        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() => SelectCharacter(characterNames[index]));
        }
    }

    private void SelectCharacter(string characterName)
    {
        if (IsClient && networkManager != null && networkManager.ClientManager.Connection != null)
        {
            SendCharacterSelectionToServer(networkManager.ClientManager.Connection, characterName);
        }
        else
        {
            Debug.LogError("NetworkManager or client connection is null.");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendCharacterSelectionToServer(NetworkConnection conn, string characterName)
    {
        if (conn == null)
        {
            Debug.LogError("Received null connection in SendCharacterSelectionToServer.");
            return;
        }

        CharacterManager.SetSelectedCharacter(conn, characterName);
    }
}
