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
            ThrowServer();
        }
    }

    [ServerRpc]
    private void ThrowServer()
    {
        weaponBase.isThrown = true;
        weaponBase.pickupTrigger.enabled = true;
        weaponBase.rb.isKinematic = false;
        weaponBase.rb.AddForce(transform.right * 17.2f, ForceMode.Impulse);
        ThrowClient();
    }

    [ObserversRpc]
    private void ThrowClient()
    {
        weaponBase.isThrown = true;
        weaponBase.pickupTrigger.enabled = true;
        weaponBase.rb.isKinematic = false;
        weaponBase.rb.AddForce(transform.right * 17.2f, ForceMode.Impulse);
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
