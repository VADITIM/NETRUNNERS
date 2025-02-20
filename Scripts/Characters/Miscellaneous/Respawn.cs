using UnityEngine;
using FishNet.Object;
using System.Collections;

public class Respawn : NetworkBehaviour
{
    private float respawnTime = 2f;
    
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider col;
    [SerializeField] private Rigidbody rb;
    
    public void DisablePlayer()
    {
        DisablePlayerServer();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisablePlayerServer()
    {
        DisablePlayerObservers();
        StartCoroutine(RespawnTimer());
    }

    [ObserversRpc]
    private void DisablePlayerObservers()
    {
        if (sprite != null) sprite.enabled = false;
        if (animator != null) animator.enabled = false;
        if (col != null) col.enabled = false;
        if (rb != null) rb.isKinematic = true;
    }

    private IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(respawnTime);
        EnablePlayerObserver();
    }

    [ObserversRpc]
    private void EnablePlayerObserver()
    {
        if (sprite != null) sprite.enabled = true;
        if (animator != null) animator.enabled = true;
        if (col != null) col.enabled = true;
        if (rb != null) rb.isKinematic = false;
    }
}