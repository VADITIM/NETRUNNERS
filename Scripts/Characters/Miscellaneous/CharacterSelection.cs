using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class CharacterSelection : NetworkBehaviour
{
    [SerializeField] private GameObject[] characters;
    [SerializeField] private GameObject characterSelectionPanel;
    [SerializeField] private GameObject canvas;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (base.IsOwner)
        {
            canvas.SetActive(true);
        }
        else
        {
            canvas.SetActive(false); 
        }
    }

    public void SpawnSamus()
    {
        if (!IsOwner) return; 
        characterSelectionPanel.SetActive(false);
        SpawnCharacter(0, base.Owner);
    }

    public void SpawnRonin()
    {
        if (!IsOwner) return; 
        characterSelectionPanel.SetActive(false);
        SpawnCharacter(1, base.Owner);
    }

    public void SpawnBiker()
    {
        if (!IsOwner) return; 
        characterSelectionPanel.SetActive(false);
        SpawnCharacter(2, base.Owner);
    }

    public void SpawnShadowRunner()
    {
        if (!IsOwner) return; 
        characterSelectionPanel.SetActive(false);
        SpawnCharacter(3, base.Owner);
    }

    public void SpawnCyberBee()
    {
        if (!IsOwner) return; 
        characterSelectionPanel.SetActive(false);
        SpawnCharacter(4, base.Owner);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnCharacter(int spawnIndex, NetworkConnection conn)
    {
        GameObject character = Instantiate(characters[spawnIndex], SpawnPoint.instance.transform.position, Quaternion.identity);
        
        Spawn(character.GetComponent<NetworkObject>(), conn);
    }
}
