using UnityEngine;

public class MirrorGameObject : MonoBehaviour
{
    private bool done = false;

    void Start()
    {
        if (!done)
        {
            done = true;
            RepositionObject();
        }
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

        Destroy(newObject.GetComponent<MirrorGameObject>());

        // Debug.Log("Object " + objectToModify.name + " has been repositioned to " + newPosition + " and rotated to " + newRotation.eulerAngles);
    }
}