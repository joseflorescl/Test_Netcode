using UnityEngine;

public abstract class BasePlayerAuthorityProvider : MonoBehaviour
{
    protected IPlayerAuthorityProvider authorityProvider;
    protected bool hasAuthority;

    protected virtual void Awake()
    {
        authorityProvider = GetComponent<IPlayerAuthorityProvider>();

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
    }

    protected virtual void OnDisable()
    {
        if (authorityProvider != null)
            authorityProvider.OnAuthorityChanged -= OnAuthorityChanged;
    }

    private void OnAuthorityChanged(bool value)
    {
        hasAuthority = value;
        ManageAuthority();
    }

    protected void ManageAuthority()
    {
        if (!hasAuthority)
        {
            enabled = false; //Destroy(this)?
        }
    }
}
