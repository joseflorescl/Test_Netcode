using UnityEngine;

public interface INetworkSpawnNotifier
{
    bool IsServer();
    event System.Action OnNetworkSpawned;
    event System.Action OnNetworkDespawned;
}
