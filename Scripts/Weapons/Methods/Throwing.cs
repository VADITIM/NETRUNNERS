using UnityEngine;
using FishNet.Object;

public class Throwing : NetworkBehaviour 
{
    private WeaponBase weaponBase;
    private float decelerationFactor = .98f; 

    public override void OnStartClient()
    {
        weaponBase = GetComponent<WeaponBase>();
        weaponBase.rb.isKinematic = true; 
    }

    public void FixedUpdate()
    {
        Throw();
        ApplyDeceleration();
    }

    public void Throw()
    {
        if (!weaponBase) return;

        if (base.HasAuthority && Input.GetKeyDown(KeyCode.Mouse0) && !weaponBase.isThrown)
        {
            weaponBase.isThrown = true;
            weaponBase.pickupTrigger.enabled = true;
            weaponBase.transform.SetParent(null);
            weaponBase.rb.isKinematic = false;
            ThrowServer();
        }
    }

    [ServerRpc]
    private void ThrowServer()
    {
        weaponBase.isThrown = true;
        weaponBase.pickupTrigger.enabled = true;
        weaponBase.transform.SetParent(null);
        weaponBase.rb.isKinematic = false;

        float throwDirection = weaponBase.weaponHolder.localPosition.x > 0 ? 1f : -1f;
        
        weaponBase.rb.AddForce(Vector3.right * throwDirection * 17.2f, ForceMode.Impulse);
        ThrowClient(throwDirection);
    }

    [ObserversRpc]
    private void ThrowClient(float throwDirection)
    {
        weaponBase.isThrown = true;
        weaponBase.pickupTrigger.enabled = true;
        weaponBase.transform.SetParent(null);
        weaponBase.rb.isKinematic = false;

        weaponBase.rb.AddForce(Vector3.right * throwDirection * 17.2f, ForceMode.Impulse);
    }

    private void ApplyDeceleration()
    {
        if (!weaponBase) return;

        if (weaponBase.isThrown && !weaponBase.rb.isKinematic)
        {
            weaponBase.rb.velocity *= decelerationFactor;
        }
    }
}
