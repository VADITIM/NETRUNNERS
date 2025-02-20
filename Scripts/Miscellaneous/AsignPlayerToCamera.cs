using UnityEngine;
using FishNet.Object;

public class AsignPlayerToCamera : NetworkBehaviour
{
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        NotifyCameraLogic();
    }

    private void NotifyCameraLogic()
    {
        CameraLogic cameraLogic = Camera.main.GetComponent<CameraLogic>();
        
        if (cameraLogic != null)
            cameraLogic.AssignPlayerDynamically(GetComponent<NetworkObject>());
        else
            return;
    }
}
