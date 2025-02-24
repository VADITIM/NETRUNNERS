using TMPro;
using UnityEngine;

public class PlayerKilledUI : MonoBehaviour
{
    [SerializeField] private CameraLogic cameraLogic;
    [SerializeField] private TextMeshProUGUI playerKilledText;
    [SerializeField] private GameObject panel;

    void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        HandlePlayerKillUI();
    }

    private void HandlePlayerKillUI()
    {
        if (cameraLogic == null) return;

        bool isPlayer1Assigned = cameraLogic.player1 != null;
        bool isPlayer2Assigned = cameraLogic.player2 != null;

        if (!isPlayer1Assigned && !isPlayer2Assigned)
        {
            panel.SetActive(false);
        }
        else if (!isPlayer1Assigned)
        {
            panel.SetActive(true);
            SetPlayerKilledText("Player 1 Killed!");
        }
        else if (!isPlayer2Assigned)
        {
            panel.SetActive(true);
            SetPlayerKilledText("Player 2 Killed!");
        }
        else
        {
            panel.SetActive(false);
        }
    }

    private void SetPlayerKilledText(string text)
    {
        playerKilledText.text = text;
    }
}