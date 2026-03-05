using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class TestRelay : MonoBehaviour
{
    const string ConnectionType = "udp"; // dtls udp
    [SerializeField] int maxPlayers = 4;
    [SerializeField] string joinCode;

    public event Action<string> OnRelayCreated;

    string PlayerId => AuthenticationService.Instance.PlayerId;


    private async void Start()
    {
        await InitializeUnityAuthentication();
    }

    private async Task InitializeUnityAuthentication()
    {
        //TODO: se puede pasar un argumento InitializationOptions para poder probar multiples build en la misma máquina.
        await UnityServices.InitializeAsync(); // await para que no se quede pegado esperando iniciar los servicios.
        AuthenticationService.Instance.SignedIn += SignedIn;
        await AuthenticationService.Instance.SignInAnonymouslyAsync(); // nos podemos autenticar de forma anónima.
    }

    private void SignedIn()
    {
        print($"Usuario conectado: {PlayerId}");
    }   

    [ContextMenu("CreateRelay")]
    public async void CreateRelay()
    {
        string relayCode = await CreateRelayAsync();
    }

    public async Task<string> CreateRelayAsync()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1); // el argumento de maxConnections es para las conexiones extras, aparte de la del host.
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            print($"joinCode: {joinCode}");

            SetRelayServerData(allocation);
            NetworkManager.Singleton.StartHost(); // Después del CreateAllocationAsync no deben pasar muchos segundos antes de hacer un StartHost.

            OnRelayCreated?.Invoke(joinCode);

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            print($"CreateRelay.RelayServiceException: {e}");
        }
        catch (System.Exception e)
        {
            print($"CreateRelay.Exception: {e}");
        }

        return null;
    }

    void SetRelayServerData(Allocation allocation)
    {
        RelayServerData relayServerData = new RelayServerData(allocation, ConnectionType);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        //NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
        //    allocation.RelayServer.IpV4,
        //    (ushort)allocation.RelayServer.Port,
        //    allocation.AllocationIdBytes,
        //    allocation.Key,
        //    allocation.ConnectionData
        //    );

    }

    [ContextMenu("JoinRelay")]
    void JoinRelay()
    {
        JoinRelayAsync(joinCode);
    }

    public void JoinRelay(string joinCode)
    {
        JoinRelayAsync(joinCode);
    }

    async void JoinRelayAsync(string joinCode)
    {
        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            print($"Joinning relay with code: {joinCode}");
            SetRelayServerData(joinAllocation);
            NetworkManager.Singleton.StartClient();
        }
        catch (System.Exception e)
        {
            print($"JoinRelay.Exception: {e}");
        }
        
    }

    private void SetRelayServerData(JoinAllocation joinAllocation)
    {
        RelayServerData relayServerData = new RelayServerData(joinAllocation, ConnectionType);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        //NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
        //    joinAllocation.RelayServer.IpV4,
        //    (ushort)joinAllocation.RelayServer.Port,
        //    joinAllocation.AllocationIdBytes,
        //    joinAllocation.Key,
        //    joinAllocation.ConnectionData,
        //    joinAllocation.HostConnectionData
        //    );
    }
}
