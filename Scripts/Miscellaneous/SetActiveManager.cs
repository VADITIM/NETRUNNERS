using UnityEngine;

public class SetActiveManager : MonoBehaviour
{
    public GameObject manager;

    public void Start()
    {
        manager.SetActive(true);
    }

}
