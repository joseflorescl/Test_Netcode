using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlayerType { None, Cross, Circle }

public class GameManager : NetworkBehaviour
{    
    public static GameManager Instance { get; private set; }
    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;
    public class OnClickedOnGridPositionEventArgs: EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }    

    PlayerType localPlayerType;
    PlayerType currentPlayablePlayerType;

    public PlayerType LocalPlayerType => localPlayerType;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Más de 1 instancia de GameManager");
        }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        print($"Local Id: {NetworkManager.Singleton.LocalClientId}");
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            localPlayerType = PlayerType.Cross;
        }
        else 
        {
            localPlayerType = PlayerType.Circle;
        }

        if (IsServer)
        {
            currentPlayablePlayerType = PlayerType.Cross;
        }
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y, PlayerType playerType)
    {
        print($"Click: {x}, {y}");

        if (playerType != currentPlayablePlayerType)
            return;

        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs
        {
            x = x,
            y = y,
            playerType = playerType
        });

        SwitchCurrentPlayer();
    }

    void SwitchCurrentPlayer()
    {
        switch (currentPlayablePlayerType)
        {
            default:
            case PlayerType.Cross:
                currentPlayablePlayerType = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                currentPlayablePlayerType = PlayerType.Cross;
                break;
        }
    }



}
