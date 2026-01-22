using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlayerType { None, Cross, Circle }

public class GameManager : NetworkBehaviour
{    
    public static GameManager Instance { get; private set; }
    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;
    public event EventHandler OnGameStarted;
    public event EventHandler OnCurrentPlayablePlayerTypeChanged;

    public class OnClickedOnGridPositionEventArgs: EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }    

    PlayerType localPlayerType;
    NetworkVariable<PlayerType> currentPlayablePlayerType = new(value: PlayerType.None);

    public PlayerType LocalPlayerType => localPlayerType;
    public PlayerType CurrentPlayablePlayerType => currentPlayablePlayerType.Value;

    PlayerType[,] playerTypeArray;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Más de 1 instancia de GameManager");
        }
        Instance = this;
        playerTypeArray = new PlayerType[3, 3];
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
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        }

        currentPlayablePlayerType.OnValueChanged += (PlayerType oldPlayerType, PlayerType newPlayerType) =>
        {
            OnCurrentPlayablePlayerTypeChanged?.Invoke(this, EventArgs.Empty);
        };

    }

    private void OnClientConnectedCallback(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            currentPlayablePlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y, PlayerType playerType)
    {
        print($"Click: {x}, {y}");

        if (playerType != currentPlayablePlayerType.Value)
            return;


        if (playerTypeArray[x, y] != PlayerType.None)
            return;

        playerTypeArray[x, y] = playerType;

        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs
        {
            x = x,
            y = y,
            playerType = playerType
        });

        SwitchCurrentPlayer();
        TestWinner();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    void SwitchCurrentPlayer()
    {
        switch (currentPlayablePlayerType.Value)
        {
            default:
            case PlayerType.Cross:
                currentPlayablePlayerType.Value = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                currentPlayablePlayerType.Value = PlayerType.Cross;
                break;
        }
    }

    void TestWinner()
    {
        if (TestWinnerLine(0))// bottom row
        {
            print("Winner");
        }
    }

    bool TestWinnerLine(int lineIdx)
    {

        var firstValue = playerTypeArray[0, lineIdx];
        if (firstValue == PlayerType.None)
            return false;

        for (int i = 1; i < playerTypeArray.GetLength(1); i++)
        {
            var item = playerTypeArray[i, lineIdx];
            if (item != firstValue)
                return false;
        }

        return true;
    }


}
