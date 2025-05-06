using UnityEngine;
using FishNet.Object;
using System.Collections;

public class Respawn : NetworkBehaviour
{
    private float respawnTime = 3.2f;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject weaponHolder;
    float xOffset;

    private CameraLogic cameraLogic;
    private CharacterBase characterBase;
    private bool isDead = false;

    private void Start()
    {
        cameraLogic = FindObjectOfType<CameraLogic>();
        characterBase = GetComponent<CharacterBase>();
    }

    private void Update()
    {
        if (isDead)
        {
        }
    }

    public void DisablePlayer()
    {
        if (!isDead)
        {
            DisablePlayerServer();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisablePlayerServer()
    {
        HandleKill();
        isDead = true;
        StartCoroutine(RespawnTimer());
    }

    private IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(respawnTime);
        RespawnPlayerServer();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RespawnPlayerServer()
    {
        HandleRespawn();
        isDead = false;
    }

    [ObserversRpc]
    private void HandleKill()
    {
        cameraLogic.RemovePlayer(GetComponent<NetworkObject>());
        if (weaponHolder != null) weaponHolder.SetActive(false);
        if (sprite != null) sprite.enabled = false;
    }

    [ObserversRpc]
    private void HandleRespawn()
    {
        NetworkObject thisPlayerNetworkObject = GetComponent<NetworkObject>();
        Vector3 cameraPlayerPosition = Vector3.zero;
        bool cameraPlayerFound = false;

        if (cameraLogic.player1 != null && cameraLogic.player2 != null)
        {
            if (thisPlayerNetworkObject.ObjectId == cameraLogic.player1ID)
            {
                cameraPlayerPosition = cameraLogic.player2.position;
                cameraPlayerFound = true;
                xOffset = (cameraLogic.player1.position.x < cameraLogic.player2.position.x) ? 15 : -15;
            }
            else if (thisPlayerNetworkObject.ObjectId == cameraLogic.player2ID)
            {
                cameraPlayerPosition = cameraLogic.player1.position;
                cameraPlayerFound = true;
                xOffset = (cameraLogic.player2.position.x > cameraLogic.player1.position.x) ? -15 : 15;
            }
        }
        else if (cameraLogic.player1 != null)
        {
            cameraPlayerPosition = cameraLogic.player1.position;
            cameraPlayerFound = true;
            xOffset = 15;
        }
        else if (cameraLogic.player2 != null)
        {
            cameraPlayerPosition = cameraLogic.player2.position;
            cameraPlayerFound = true;
            xOffset = -15; 
        }

        Vector3 respawnPosition = cameraPlayerPosition;
        if (cameraPlayerFound)
        {
            respawnPosition.x += xOffset;
        }

        transform.position = respawnPosition;

        cameraLogic.AssignPlayerDynamically(thisPlayerNetworkObject);
        if (weaponHolder != null) weaponHolder.SetActive(true);
        if (sprite != null) sprite.enabled = true;

        // Reset the weapon state and position
        ResetWeaponServer();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResetWeaponServer()
    {
        ResetWeaponClient();
    }

    [ObserversRpc]
    private void ResetWeaponClient()
    {
        if (characterBase.weaponBase != null)
        {
            WeaponBase weapon = characterBase.weaponBase;
            weapon.isBroken = false;
            weapon.isThrown = false;
            weapon.rb.isKinematic = true;
            weapon.pickupCollider.enabled = false;

            weapon.transform.SetParent(weaponHolder.transform);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
        }
    }
}