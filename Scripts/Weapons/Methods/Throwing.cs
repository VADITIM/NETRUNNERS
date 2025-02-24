using UnityEngine;
using FishNet.Object;

public class Throwing : NetworkBehaviour 
{
    private WeaponBase weaponBase;
    private float decelerationFactor = 0.985f; 
    private float initialThrowSpeed = 37.2f;
    private float gravityFactor = -.81f;
    private float groundCheckDistance = 0.2f;

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
        
        weaponBase.rb.velocity = new Vector3(throwDirection * initialThrowSpeed, 0, gravityFactor);
        ThrowClient(throwDirection);
    }

    [ObserversRpc]
    private void ThrowClient(float throwDirection)
    {
        weaponBase.isThrown = true;
        weaponBase.pickupTrigger.enabled = true;
        weaponBase.transform.SetParent(null);
        weaponBase.rb.isKinematic = false;

        weaponBase.rb.velocity = new Vector3(throwDirection * initialThrowSpeed, 0, gravityFactor);
    }

    private void ApplyDeceleration()
    {
        if (!weaponBase) return;

        if (weaponBase.isThrown && !weaponBase.rb.isKinematic)
        {
            RaycastHit hit;
            if (Physics.Raycast(weaponBase.transform.position, Vector3.down, out hit, groundCheckDistance))
            {
                weaponBase.rb.velocity = new Vector3(
                    weaponBase.rb.velocity.x * decelerationFactor,
                    Mathf.Max(0, weaponBase.rb.velocity.y),
                    weaponBase.rb.velocity.z
                );
            }
            else
            {
                weaponBase.rb.velocity = new Vector3(
                    weaponBase.rb.velocity.x * decelerationFactor,
                    weaponBase.rb.velocity.y + gravityFactor * Time.fixedDeltaTime,
                    weaponBase.rb.velocity.z
                );
            }
        }
    }
}