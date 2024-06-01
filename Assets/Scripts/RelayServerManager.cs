using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class RelayServerManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI joinCodeText;
    [SerializeField] private TMP_InputField joinCodeInput;
    //[SerializeField] private int maxPlayers = 2;

    private NetworkManager networkManager;

    public async void Start()
    {
        
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void StartRelay(){
        string joinCode = await StartHostRelay();
        joinCodeText.text = joinCode ;
    }

    private async Task<string> StartHostRelay(int maxPlayers = 2){
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    public async void JoinRelay(){
        await StartClientRelay(joinCodeInput.text);
    }

    private async Task<bool> StartClientRelay(string joinCode){

        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }
}