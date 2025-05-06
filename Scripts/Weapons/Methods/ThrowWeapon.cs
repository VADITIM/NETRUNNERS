using UnityEngine;
using FishNet.Object;

public class ThrowWeapon : NetworkBehaviour
{
    private WeaponBase weaponBase;

    private float decelerationFactor = 0.985f;
    private float initialThrowSpeed = 10.2f;
    private float gravityFactor = -0.81f;
    private float groundCheckDistance = 0.2f;
    public bool groundCheck = false;

    private BoxCollider hitbox;

    public override void OnStartClient()
    {
        hitbox = GetComponent<BoxCollider>();
        weaponBase = GetComponent<WeaponBase>();
        weaponBase.rb.isKinematic = true;
    }

    public void Update()
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
            weaponBase.pickupCollider.enabled = true;
            weaponBase.transform.SetParent(null);
            weaponBase.rb.isKinematic = false;
            ThrowServer();
        }
    }

    [ServerRpc]
    private void ThrowServer()
    {
        weaponBase.isThrown = true;
        weaponBase.pickupCollider.enabled = true;
        weaponBase.transform.SetParent(null);
        weaponBase.rb.isKinematic = false;

        float throwDirection = weaponBase.weaponHolder.localPosition.x > 0 ? 1f : -1f;

        weaponBase.rb.velocity = new Vector3(throwDirection * initialThrowSpeed, 0, gravityFactor);
        // CheckIfWeaponBreaks();
        ThrowClient(throwDirection);
    }

    [ObserversRpc]
    private void ThrowClient(float throwDirection)
    {
        weaponBase.isThrown = true;
        weaponBase.pickupCollider.enabled = true;
        weaponBase.transform.SetParent(null);
        weaponBase.rb.isKinematic = false;

        weaponBase.rb.velocity = new Vector3(throwDirection * initialThrowSpeed, 0, gravityFactor);
        // CheckIfWeaponBreaks();
    }

    private void ApplyDeceleration()
    {
        if (!weaponBase) return;

        if (weaponBase.isThrown && !weaponBase.rb.isKinematic)
        {
            RaycastHit hit;
            if (Physics.Raycast(weaponBase.transform.position, Vector3.down, out hit, groundCheckDistance))
            {
                weaponBase.rb.velocity = new Vector3(weaponBase.rb.velocity.x * decelerationFactor, 0, weaponBase.rb.velocity.z);
                groundCheck = true;
                hitbox.enabled = false;

                // if (weaponBase.isBroken)
                // {
                //     WeaponBrokenServer();
                // }
            }
            else
            {
                groundCheck = false;
                hitbox.enabled = true;

                weaponBase.rb.velocity += new Vector3(0, gravityFactor * Time.fixedDeltaTime, 0);
            }
        }
    }

    private void CheckIfWeaponBreaks()
    {
        float chance = Random.Range(0f, 1f);
        if (chance <= 0.2f)
        {
            weaponBase.isBroken = true;
            Debug.Log("Weapon has broken!");
        }
    }

    [ServerRpc]
    private void WeaponBrokenServer()
    {
        weaponBase.isBroken = true;
        WeaponBrokenClient();
    }

    [ObserversRpc]
    private void WeaponBrokenClient()
    {
        weaponBase.isThrown = false;
        weaponBase.pickupCollider.enabled = false;
        weaponBase.transform.SetParent(weaponBase.weaponHolder);
        weaponBase.transform.localPosition = Vector3.zero;
        weaponBase.transform.localRotation = Quaternion.identity;

        weaponBase.gameObject.SetActive(false);
    }
}