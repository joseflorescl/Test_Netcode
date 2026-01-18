using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ClientPlayerNetwork : NetworkBehaviour, IPlayerAuthorityProvider
{
    public bool HasPlayerAuthority => IsOwner;
    public event System.Action<bool> OnAuthorityChanged;


    public override void OnNetworkSpawn()
    {
        Debug.Log($"PlayerNetwork OnNetworkSpawn: {name}. IsOwner: {IsOwner}", gameObject);
        OnAuthorityChanged?.Invoke(HasPlayerAuthority);
    }

    public override void OnNetworkDespawn()
    {
        OnAuthorityChanged?.Invoke(false);
    }
}
