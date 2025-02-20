using UnityEngine;

public class SetActive : MonoBehaviour
{
    public GameObject manager;

    public void Start()
    {
        manager.SetActive(true);
    }

}
