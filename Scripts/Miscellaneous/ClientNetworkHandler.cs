using UnityEngine;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using FishNet.Example;

public class ClientNetworkHandler : NetworkBehaviour
{
    public NetworkManager networkManager;
    public GameObject NetworkCanvas;
    public bool isNetworkManagerDisabled = false;

    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        NetworkCanvas = networkManager.GetComponentInChildren<NetworkHudCanvases>().gameObject;

        if (networkManager.ClientManager == null) return;
            networkManager.ClientManager.OnClientConnectionState += OnClientConnectionStateChanged;
    }

    private void OnDestroy()
    {
        if (networkManager == null && networkManager.ClientManager == null) return;
            networkManager.ClientManager.OnClientConnectionState -= OnClientConnectionStateChanged;
    }

    public void OnPlayerAssigned(NetworkObject playerObject)
    {
        if (isNetworkManagerDisabled || playerObject == null) return;

        DisableNetworkManager();
    }

    private void DisableNetworkManager()
    {
        DisableNetworkManagerServer();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisableNetworkManagerServer()
    {
        NetworkCanvas.SetActive(false);
        DisableNetworkManagerClient();
    }

    [ObserversRpc]
    private void DisableNetworkManagerClient()
    {
        NetworkCanvas.SetActive(false);
    }
    
    private void OnClientConnectionStateChanged(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started) { }
    }
}