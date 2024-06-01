using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Timer : NetworkBehaviour
{

    public float countdownTime = 20f; // in seconds
    public bool LevelCompleted = false;
    public TextMeshProUGUI displayText;
    private int requiredPlayers = 2; // Set this to the number of players required to start the countdown
    private int connectedPlayers = 0;
    private bool countdownStarted = false;
    private NetworkVariable<float> networkedCountdownTime = new NetworkVariable<float>();

    public override void OnNetworkSpawn(){
        if (!IsServer){
            enabled = false;
            return;
        }

        networkedCountdownTime.Value = countdownTime;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        
        networkedCountdownTime.OnValueChanged += OnCountdownTimeChanged;
        displayText.text = networkedCountdownTime.Value.ToString("F0");
    }

    private void OnClientConnected(ulong clientId)
    {
        connectedPlayers++;
        CheckAndStartCountdown();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        connectedPlayers--;
    }

    private void CheckAndStartCountdown()
    {
        if (connectedPlayers >= requiredPlayers && !countdownStarted)
        {
            countdownStarted = true;
            StartCoroutine(StartCountdown());
        }
    }

    private void OnCountdownTimeChanged(float oldValue, float newValue)
    {
        if (newValue <= 0)
        {
            displayText.text = "Completed";
        }
        else
        {
            displayText.text = newValue.ToString("F0");
        }
    }

    private IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            displayText.text = countdownTime.ToString("F0");
            yield return new WaitForSeconds(1f);
            countdownTime -= 1f;
        }

        LevelCompleted = true;
        displayText.text = "Completed! ESCAPE!";
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        networkedCountdownTime.OnValueChanged -= OnCountdownTimeChanged;
    }
}
