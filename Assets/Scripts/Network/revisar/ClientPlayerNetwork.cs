using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ClientPlayerNetwork : NetworkBehaviour, IPlayerAuthorityProvider, INetworkSpawnNotifier
{

    public NetworkVariable<int> TODO_TEST = new();

    // Interfaz IPlayerAuthorityProvider
    public bool HasPlayerAuthority => IsOwner;
    public event System.Action<bool> OnAuthorityChanged;

    // Interfaz INetworkSpawnNotifier
    public event System.Action OnNetworkSpawned;
    public event System.Action OnNetworkDespawned;


    public override void OnNetworkSpawn()
    {
        Debug.Log($"PlayerNetwork OnNetworkSpawn: {name}. IsOwner: {IsOwner}", gameObject);
        OnAuthorityChanged?.Invoke(HasPlayerAuthority);
        OnNetworkSpawned?.Invoke();

        TODO_TEST.Value = Random.Range(0, 100);
    }

    public override void OnNetworkDespawn()
    {
        OnAuthorityChanged?.Invoke(false);
        OnNetworkDespawned?.Invoke();
    }

    bool INetworkSpawnNotifier.IsServer()
    {
        return IsServer;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print($"Valor de TODO_TEST: {TODO_TEST.Value}");
        }
    }
}
