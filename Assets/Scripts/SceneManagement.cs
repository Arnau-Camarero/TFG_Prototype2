using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : NetworkBehaviour
{
    private NetworkManager networkManager;

    void Awake()
    {
        // Make sure this GameObject persists across scenes
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        networkManager = NetworkManager.Singleton;

        // Ensure the NetworkManager is initialized
        if (networkManager == null)
        {
            Debug.LogError("NetworkManager not found!");
            return;
        }

        // Register callback for scene load completion
        networkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
    }

    // Method to change scene, called on the server
    [ServerRpc(RequireOwnership = false)]
    public void ChangeSceneServerRpc(string sceneName)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }

    // Method to load a scene, can be called by any client
    [ClientRpc]
    public void LoadSceneClientRpc(string sceneName)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    // Callback when a scene is loaded
    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log("Scene Loaded: " + sceneName);

        if (NetworkManager.Singleton.IsServer)
        {
            // If the server loaded the scene, tell clients to load it too
            LoadSceneClientRpc(sceneName);
        }
    }

    public override void OnDestroy()
    {
        if (networkManager != null && networkManager.SceneManager != null)
        {
            networkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
        }
    }
}
