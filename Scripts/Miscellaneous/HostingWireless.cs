using UnityEngine;
using TMPro;
using System.IO;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities.Types;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using UnityEngine.UI;

public class HostingWireless : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField, Scene] public string gameScene;
    [SerializeField] private TMP_Text hostIPText;

    private string GetLocalIPv4Address()
    {
        string localIP = "Unavailable";
        
        NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
        
        foreach (NetworkInterface ni in interfaces)
        {
            if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet && 
                ni.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.Address.ToString();
                        Debug.Log("Using wired connection IP: " + localIP);
                        return localIP;
                    }
                }
            }
        }

        foreach (NetworkInterface ni in interfaces)
        {
            if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && 
                ni.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.Address.ToString();
                        Debug.Log("Using wireless connection IP: " + localIP);
                        return localIP;
                    }
                }
            }
        }

        if (localIP == "Unavailable")
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    Debug.Log("Using fallback connection IP: " + localIP);
                    break;
                }
            }
        }

        return localIP;
    }

    void Start()
    {
        inputField.text = "";
        inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "Enter IP";

        networkManager.ServerManager.OnServerConnectionState += OnServerConnectionStateCallback;
        networkManager.SceneManager.OnLoadEnd += OnSceneLoadEndCallback;
    }

    private void OnDestroy()
    {
        if (networkManager != null)
        {
            networkManager.ServerManager.OnServerConnectionState -= OnServerConnectionStateCallback;
            networkManager.SceneManager.OnLoadEnd -= OnSceneLoadEndCallback;
        }
    }

    public void OnServerConnectionStateCallback(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            Debug.Log("Server started. Loading game scene...");
            SceneLoadData sceneLoadData = new SceneLoadData(GetSceneName(gameScene))
            {
                ReplaceScenes = ReplaceOption.All
            };
            networkManager.SceneManager.LoadGlobalScenes(sceneLoadData);
        }
    }

    public void OnSceneLoadEndCallback(SceneLoadEndEventArgs args)
    {
        foreach (var scene in args.LoadedScenes)
        {
            if (scene.name == GetSceneName(gameScene))
            {
                if (networkManager.IsServerStarted && !networkManager.IsClientStarted)
                {
                    Debug.Log("Server scene loaded. Server joining as a client...");
                    networkManager.ClientManager.StartConnection();
                }
                else if (networkManager.IsClientStarted)
                {
                    Debug.Log("Client scene loaded. Client joined.");
                }
            }
        }
    }

    public void OnJoinClick()
    {
        if (networkManager == null)
        {
            Debug.LogError("Cannot start client: NetworkManager is missing.");
            return;
        }

        string enteredIP = inputField.text.Trim();

        if (!IPAddress.TryParse(enteredIP, out var ipAddress) ||
            ipAddress.AddressFamily != AddressFamily.InterNetwork)
        {
            Debug.LogError("Invalid IPv4 address entered.");

            return;
        }

        networkManager.TransportManager.Transport.SetClientAddress(enteredIP);

        networkManager.ClientManager.StartConnection();
        Debug.Log($"Attempting to join server at {enteredIP}");
    }

    public void OnHostClick()
    {
        if (networkManager == null)
        {
            Debug.LogError("Cannot start server: NetworkManager is missing.");
            return;
        }

        networkManager.ServerManager.StartConnection();
        string hostIP = GetLocalIPv4Address();
        Debug.Log("Hosting started on IP: " + hostIP);

        if (hostIPText != null)
        {
            hostIPText.text = hostIP;
        }
    }

    private string GetSceneName(string fullPath)
    {
        return Path.GetFileNameWithoutExtension(fullPath);
    }
}