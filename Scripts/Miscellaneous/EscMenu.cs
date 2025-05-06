using UnityEngine.SceneManagement;
using UnityEngine;

public class EscMenu : MonoBehaviour
{
    [SerializeField] private GameObject escMenu;
    private bool isActive = false;
    public CameraLogic cameraLogic;

    private void Update()
    {
        EscapeMenu();
    }

    private void EscapeMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isActive = !isActive;
            escMenu.SetActive(isActive);
        }
    }

    public void OnExitClick()
    {
        SceneManager.LoadScene("Lobby");
        cameraLogic.ResetCameraLogic();
    }
}
