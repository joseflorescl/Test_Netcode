using Unity.Netcode;
using UnityEngine;

public abstract class NetworkAuthorityComponentDisabler<T> : NetworkBehaviour where T : Behaviour
{
    protected T targetComponent;

    protected virtual void Awake()
    {
        targetComponent = GetComponent<T>();

        if (targetComponent == null)
        {
            Debug.LogError($"{GetType().Name}: No se encontró componente {typeof(T).Name}", gameObject);
            enabled = false;
        }
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"{nameof(T)} OnNetworkSpawn. IsOwner: {IsOwner}", gameObject);
        ApplyAuthority(IsOwner);
    }

    protected virtual void ApplyAuthority(bool isOwner)
    {
        targetComponent.enabled = isOwner;
    }
}

