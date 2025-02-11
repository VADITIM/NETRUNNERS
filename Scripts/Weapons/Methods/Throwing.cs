using FishNet.Object;
using UnityEngine;

public class Throwing : NetworkBehaviour 
{
    private WeaponBase weaponBase;
    private float decelerationFactor = .98f; // Adjust this value to control the rate of deceleration

    public override void OnStartClient()
    {
        weaponBase = GetComponent<WeaponBase>();
        weaponBase.rb.isKinematic = true; // Set the Rigidbody to be kinematic initially
    }

    public void FixedUpdate()
    {
        Throw();
        ApplyDeceleration();
    }

    public void Throw()
    {
        if (base.HasAuthority && Input.GetKeyDown(KeyCode.Mouse0) && !weaponBase.isThrown)
        {
            weaponBase.isThrown = true;
            ThrowServer();
        }
    }

    [ServerRpc]
    private void ThrowServer()
    {
        Debug.Log("Throw executed on server."); 

        weaponBase.isThrown = true;
        weaponBase.rb.isKinematic = false; // Make the Rigidbody non-kinematic when thrown
        weaponBase.rb.AddForce(transform.right * 17.2f, ForceMode.Impulse);
        ThrowClient();
    }

    [ObserversRpc]
    private void ThrowClient()
    {
        Debug.Log("Throw synced to all clients."); 

        weaponBase.isThrown = true;
        weaponBase.rb.isKinematic = false; // Make the Rigidbody non-kinematic when thrown
        weaponBase.rb.AddForce(transform.right * 17.2f, ForceMode.Impulse);
    }

    private void ApplyDeceleration()
    {
        if (weaponBase.isThrown)
        {
            weaponBase.rb.velocity *= decelerationFactor;
        }
    }
}