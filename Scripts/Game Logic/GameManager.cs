using UnityEngine;
using FishNet.Object;

public class GameManager : NetworkBehaviour
{
    public CameraLogic cameraLogic;
    public bool gameStarted = false;

    private void Start()
    {
        cameraLogic = FindObjectOfType<CameraLogic>();
        if (cameraLogic == null)
        {
            Debug.LogError("CameraLogic component not found in the scene.");
            return;
        }
    }

    private void Update()
    {
        if (!gameStarted )
        {
            StartGame();
        }
    }


    private void StartGame()
    {
        if (gameStarted) return;

        gameStarted = true;
        Debug.Log("Both players are fully connected and ready. Starting the game...");
    }
}
