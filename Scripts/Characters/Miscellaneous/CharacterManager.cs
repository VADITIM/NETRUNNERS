using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;

public class CharacterManager : MonoBehaviour
{
    private static Dictionary<NetworkConnection, string> selectedCharacters = new();

    public static void SetSelectedCharacter(NetworkConnection conn, string characterName)
    {
        if (conn != null)
        {
            selectedCharacters[conn] = characterName;
        }
    }

    public static string GetSelectedCharacter(NetworkConnection conn)
    {
        return conn != null && selectedCharacters.ContainsKey(conn) ? selectedCharacters[conn] : "DefaultCharacter";
    }
}
