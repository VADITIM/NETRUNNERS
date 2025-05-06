using UnityEngine;
using FishNet.Object;

public class GameEnd : NetworkBehaviour
{
    [SerializeField] private GameObject gameEndUI;
    [SerializeField] private Collider gameEndCollider;

    private void OnTriggerEnter(Collider player)
    {
        if ((player.CompareTag("Player1") && gameEndCollider.CompareTag("GameEnd1")) ||
            (player.CompareTag("Player2") && gameEndCollider.CompareTag("GameEnd2")))
        {
            TriggerGameEndServer();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TriggerGameEndServer()
    {
        TriggerGameEndClient();
    }

    [ObserversRpc]
    private void TriggerGameEndClient()
    {
        gameEndUI.SetActive(true);
    }

    public void OnExitGameClick()
    {
        Application.Quit();
    }
}