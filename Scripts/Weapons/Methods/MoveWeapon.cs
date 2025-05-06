using UnityEngine;
using FishNet.Object;

public class MoveWeapon : NetworkBehaviour
{
    public float lastPosY { get; private set; }
    
    private const float SNAPDISTANCE = .5f;
    private const float MAXMOVEDISTANCE = .5f;
    private float currentYOffset = 0f;
    
    private WeaponBase weaponBase;

    public override void OnStartClient()
    {
        weaponBase = GetComponent<WeaponBase>();
    }

    public void Update()
    {
        Move();
    }

    public void Move()
    {
        if (!weaponBase || !base.HasAuthority || weaponBase.weaponHolder == null || weaponBase.owner == null) return;
        
        float baseY = weaponBase.owner.transform.position.y + .8f;

        if (Input.GetKeyDown(KeyCode.W) && currentYOffset < MAXMOVEDISTANCE)
        {
            currentYOffset += SNAPDISTANCE;
        }
        else if (Input.GetKeyDown(KeyCode.S) && currentYOffset > -MAXMOVEDISTANCE)
        {
            currentYOffset -= SNAPDISTANCE;
        }

        lastPosY = baseY + currentYOffset;
        MoveServer(lastPosY);
    }

    [ServerRpc]
    private void MoveServer(float newYPosition)
    {
        if (!weaponBase || weaponBase.weaponHolder == null) return;

        lastPosY = newYPosition;
        weaponBase.weaponHolder.position = new Vector3(weaponBase.weaponHolder.position.x, newYPosition, weaponBase.weaponHolder.position.z);

        MoveClient(newYPosition);
    }

    [ObserversRpc]
    private void MoveClient(float newYPosition)
    {
        if (!weaponBase || weaponBase.weaponHolder == null) return;

        lastPosY = newYPosition;
        weaponBase.weaponHolder.position = new Vector3(weaponBase.weaponHolder.position.x, newYPosition, weaponBase.weaponHolder.position.z);
    }
}
