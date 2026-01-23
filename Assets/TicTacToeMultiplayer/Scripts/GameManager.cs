using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum PlayerType { None, Cross, Circle }

public class GameManager : NetworkBehaviour
{    
    public static GameManager Instance { get; private set; }
    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;
    public event EventHandler OnGameStarted;
    public event EventHandler OnCurrentPlayablePlayerTypeChanged;
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public event EventHandler OnRematch;
    public event EventHandler OnGameTie;
    public event EventHandler OnScoreChanged;
    public event EventHandler OnPlacedObject;

    public class OnGameWinEventArgs : EventArgs
    {
        public Line line;
        public PlayerType winPlayerType;
    }

    public class OnClickedOnGridPositionEventArgs: EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }

    public enum LineType { Horizontal, Vertical, Diagonal }
    public struct Line
    {
        public LineType lineType;
        public int lineIndex;
        public Vector2Int centerGridPosition;
    }

    PlayerType localPlayerType;
    NetworkVariable<PlayerType> currentPlayablePlayerType = new(value: PlayerType.None);

    public PlayerType LocalPlayerType => localPlayerType;
    public PlayerType CurrentPlayablePlayerType => currentPlayablePlayerType.Value;

    PlayerType[,] playerTypeArray;
    List<Line> lineList;
    NetworkVariable<int> playerCrossScore = new();
    NetworkVariable<int> playerCircleScore = new();


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Más de 1 instancia de GameManager");
        }
        Instance = this;
        playerTypeArray = new PlayerType[3, 3];

        InitLineList();
    }

    void InitLineList()
    {
        lineList = new List<Line>()
        {
            new Line() { lineType = LineType.Horizontal, lineIndex = 0, centerGridPosition = new Vector2Int(1,0) },
            new Line() { lineType = LineType.Horizontal, lineIndex = 1, centerGridPosition = new Vector2Int(1,1) },
            new Line() { lineType = LineType.Horizontal, lineIndex = 2, centerGridPosition = new Vector2Int(1,2) },
            //
            new Line() { lineType = LineType.Vertical, lineIndex = 0, centerGridPosition = new Vector2Int(0,1) },
            new Line() { lineType = LineType.Vertical, lineIndex = 1, centerGridPosition = new Vector2Int(1,1) },
            new Line() { lineType = LineType.Vertical, lineIndex = 2, centerGridPosition = new Vector2Int(2,1) },
            //
            new Line() { lineType = LineType.Diagonal, lineIndex = 0, centerGridPosition = new Vector2Int(1,1) },
            new Line() { lineType = LineType.Diagonal, lineIndex = 2, centerGridPosition = new Vector2Int(1,1) },
        };
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

        playerCrossScore.OnValueChanged += (int prevScore, int newScore)  => { OnScoreChanged?.Invoke(this, EventArgs.Empty); };
        playerCircleScore.OnValueChanged += (int prevScore, int newScore) => { OnScoreChanged?.Invoke(this, EventArgs.Empty); };

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
        TriggerOnPlacedObjectRpc();

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
    void TriggerOnPlacedObjectRpc()
    {
        OnPlacedObject?.Invoke(this, EventArgs.Empty);
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
        for (int i = 0; i < lineList.Count; i++)
        {
            Line line = lineList[i];

            if (TestWinnerLine(line.lineIndex, line.lineType))
            {
                print("Winner");
                currentPlayablePlayerType.Value = PlayerType.None;
                PlayerType winner = playerTypeArray[line.centerGridPosition.x, line.centerGridPosition.y];

                switch (winner)
                {                    
                    case PlayerType.Cross:
                        playerCrossScore.Value++;
                        break;
                    case PlayerType.Circle:
                        playerCircleScore.Value++;
                        break;
                }

                TriggerOnGameWinRpc(i, winner);
                return;
            }
        }

        // Validar un empate
        bool hasTie = true;
        for (int i = 0; i < playerTypeArray.GetLength(0); i++)
        {
            for (int j = 0; j < playerTypeArray.GetLength(1); j++)
            {
                if (playerTypeArray[i, j] == PlayerType.None)
                {
                    hasTie = false;
                    break;
                }

            }
        }

        if (hasTie)
        {
            TriggerOnGamedTieRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void TriggerOnGamedTieRpc()
    {
        OnGameTie?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void TriggerOnGameWinRpc(int lineIndex, PlayerType winPlayerType)
    {
        Line line = lineList[lineIndex];
        OnGameWin?.Invoke(this, new OnGameWinEventArgs
        {
            line = line,
            winPlayerType = winPlayerType
        });
    }

    bool TestWinnerLine(int lineIdx, LineType lineType)
    {

        PlayerType firstValue;
        switch (lineType)
        {
            default:
            case LineType.Horizontal:
            case LineType.Diagonal:
                firstValue = playerTypeArray[0, lineIdx];
                break;
            case LineType.Vertical:
                firstValue = playerTypeArray[lineIdx, 0];
                break;
        }
        
        if (firstValue == PlayerType.None)
            return false;

        if (lineType == LineType.Horizontal || lineType == LineType.Vertical)
        {
            for (int i = 1; i < playerTypeArray.GetLength(1); i++) // Como la grilla es de 3x3 basta con un solo for.
            {
                PlayerType item;
                switch (lineType)
                {
                    default:
                    case LineType.Horizontal:
                        item = playerTypeArray[i, lineIdx];
                        break;
                    case LineType.Vertical:
                        item = playerTypeArray[lineIdx, i];
                        break;
                }
                if (item != firstValue)
                    return false;
            }
        }
        else // Diagonal
        {
            int currentY = lineIdx;
            int deltaY = lineIdx == 0 ? +1 : -1;
            for (int i = 1; i < playerTypeArray.GetLength(1); i++)
            {
                currentY = currentY + deltaY;
                var item = playerTypeArray[i, currentY];
                if (item != firstValue)
                    return false;
            }
        }

        return true;
    }

    [Rpc(SendTo.Server)]
    public void RematchRpc()
    {
        for (int i = 0; i < playerTypeArray.GetLength(0); i++)
        {
            for (int j = 0; j < playerTypeArray.GetLength(1); j++)
            {
                playerTypeArray[i, j] = PlayerType.None;
            }
        }
        currentPlayablePlayerType.Value = PlayerType.Cross;

        TriggerOnRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void TriggerOnRematchRpc()
    {
        OnRematch?.Invoke(this, EventArgs.Empty);
    }

    public (int playerCrossScore, int playerCircleScore) GetScores()
    {
        return new()
        {
            playerCrossScore = this.playerCrossScore.Value,
            playerCircleScore = this.playerCircleScore.Value
        };
    }

}
