// using UnityEngine;
// using UnityEngine.UI;
// using FishNet.Connection;
// using FishNet.Managing;
// using FishNet.Object;

// public class CharacterSelectionUI : NetworkBehaviour
// {
//     [SerializeField] private GameObject selectionPanel;
//     [SerializeField] private Button[] characterButtons;
//     [SerializeField] private string[] characterNames;
    
//     private NetworkManager networkManager;

//     private void Start()
//     {
//         networkManager = FindObjectOfType<NetworkManager>();
//         selectionPanel.SetActive(true);

//         for (int i = 0; i < characterButtons.Length; i++)
//         {
//             int index = i;
//             characterButtons[i].onClick.AddListener(() => SelectCharacter(characterNames[index]));
//         }
//     }

//     private void SelectCharacter(string characterName)
//     {
//         if (IsOwner)
//         {
//             SendCharacterSelectionToServer(characterName);
//         }
//         else
//         {
//             Debug.LogError("Not the owner of this UI.");
//         }
//     }

//     [ServerRpc(RequireOwnership = false)]
//     private void SendCharacterSelectionToServer(string characterName, NetworkConnection conn = null)
//     {
//         if (conn == null)
//             conn = Owner; // FishNet automatically assigns the sender's connection

//         CharacterManager.SetSelectedCharacter(conn, characterName);
//         Debug.Log($"Character {characterName} selected for player {conn.ClientId}");
//     }
// }
