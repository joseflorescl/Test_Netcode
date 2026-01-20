using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerCubeMovementNetwork : NetworkBehaviour
{
    PlayerCubeMovement playerCubeMovement;

    private void Awake()
    {
        playerCubeMovement = GetComponent<PlayerCubeMovement>();
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"PlayerCubeMovementNetwork OnNetworkSpawn. IsOwner: {IsOwner}", gameObject);
        ManageAuthority(IsOwner);
    }

    void ManageAuthority(bool isOwner)
    {
        if (!isOwner)
        {
            playerCubeMovement.enabled = false;
        }
    }
}
