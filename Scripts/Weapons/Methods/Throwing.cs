using FishNet.Object;
using UnityEngine;

public class Throwing : NetworkBehaviour 
{
    private WeaponBase weaponBase;

    public override void OnStartClient()
    {
        weaponBase = GetComponent<WeaponBase>();
    }

    public void FixedUpdate()
    {
        Throw();
        ThrowDown();
    }

    public void Throw()
    {
        if (base.HasAuthority && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowServer();
        }
    }

    [ServerRpc]
    private void ThrowServer()
    {
        Debug.Log("Throw executed on server."); 

        weaponBase.isThrown = true;
        weaponBase.rb.AddForce(transform.up * 0.2f, ForceMode.Impulse);
        ThrowClient();
    }

    [ObserversRpc]
    private void ThrowClient()
    {
        Debug.Log("Throw synced to all clients."); 

        weaponBase.isThrown = true;
        weaponBase.rb.AddForce(transform.up * 0.2f, ForceMode.Impulse);
    }

    
    public void ThrowDown()
    {
        if (base.HasAuthority && Input.GetKeyDown(KeyCode.Mouse1))
        {
            ThrowDownServer();
        }
    }

    [ServerRpc]
    private void ThrowDownServer()
    {
        weaponBase.rb.AddForce(-transform.up * .2f, ForceMode.Impulse);

        ThrowDownClient();
    }

    [ObserversRpc]
    private void ThrowDownClient()
    {
        weaponBase.rb.AddForce(-transform.up * .2f, ForceMode.Impulse);
    }
}
