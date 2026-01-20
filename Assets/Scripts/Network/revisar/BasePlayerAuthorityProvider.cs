using UnityEngine;

public abstract class BasePlayerAuthorityProvider : MonoBehaviour
{
    protected IPlayerAuthorityProvider authorityProvider;
    protected bool hasAuthority;
    protected INetworkSpawnNotifier notifier;

    protected virtual void Awake()
    {
        authorityProvider = GetComponent<IPlayerAuthorityProvider>();
        notifier = GetComponent<INetworkSpawnNotifier>();

        // Permitir singleplayer, y como OnNetworkSpawn puede ocurrir antes:
        if (authorityProvider == null)
        {
            hasAuthority = true;
            ManageAuthority();
        }
    }

    protected virtual void OnEnable()
    {
        if (authorityProvider != null)
            authorityProvider.OnAuthorityChanged += OnAuthorityChanged;

        if (notifier != null)
        {
            notifier.OnNetworkSpawned += OnNetworkSpawned;
            notifier.OnNetworkDespawned += OnNetworkDespawned;
        }
    }

    protected virtual void OnDisable()
    {
        if (authorityProvider != null)
            authorityProvider.OnAuthorityChanged -= OnAuthorityChanged;

        if (notifier != null)
        {
            notifier.OnNetworkSpawned -= OnNetworkSpawned;
            notifier.OnNetworkDespawned -= OnNetworkDespawned;
        }

    }

    private void OnAuthorityChanged(bool value)
    {
        hasAuthority = value;
        ManageAuthority();
    }

    protected virtual void OnNetworkSpawned() { }

    protected virtual void OnNetworkDespawned() { }

    protected virtual bool IsServer()
    {
        return notifier.IsServer();
    }

    protected virtual void ManageAuthority()
    {
        if (!hasAuthority)
        {
            enabled = false; //Destroy(this)?
        }
    }
}
