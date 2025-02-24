using UnityEngine;

public class MirrorGameObject : MonoBehaviour
{
    private bool done = false;

    private GameObject parentObject;

    void Start()
    {
        parentObject = GameObject.Find("Game Field");

        if (done) return;
        done = true;
        RepositionObject();
    }

    private void RepositionObject()
    {
        GameObject objectToModify = this.gameObject;

        Vector3 newPosition = objectToModify.transform.position;
        newPosition.x *= -1;

        Quaternion newRotation = objectToModify.transform.rotation;
        newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y - 180, newRotation.eulerAngles.z);

        GameObject newObject = Instantiate(objectToModify, newPosition, newRotation);
        newObject.name = objectToModify.name + "_Copy";

        if (parentObject != null)
        {
            newObject.transform.SetParent(parentObject.transform);
        }

        Destroy(newObject.GetComponent<MirrorGameObject>());
    }
}