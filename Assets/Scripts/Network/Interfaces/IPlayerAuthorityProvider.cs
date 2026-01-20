using UnityEngine;

public interface IPlayerAuthorityProvider
{
    bool HasPlayerAuthority { get; }
    event System.Action<bool> OnAuthorityChanged;

}
