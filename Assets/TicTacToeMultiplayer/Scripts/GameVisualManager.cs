using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    private const float GRID_SIZE = 4f;

    [SerializeField] Transform crossPrefab;
    [SerializeField] Transform circlePrefab;
    [SerializeField] Transform lineCompletePrefab;


    private void Start()
    {
        GameManager.Instance.OnClickedOnGridPosition += OnClickedOnGridPosition;
        GameManager.Instance.OnGameWin += OnGameWin;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnClickedOnGridPosition -= OnClickedOnGridPosition;
        GameManager.Instance.OnGameWin -= OnGameWin;
    }

    private void OnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionEventArgs e)
    {
        SpawnObjectRpc(e.x, e.y, e.playerType);
    }

    private void OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        float eulerZ = 0;
        switch (e.line.lineType)
        {
            case GameManager.LineType.Horizontal:
                eulerZ = 0;
                break;
            case GameManager.LineType.Vertical:
                eulerZ = 90;
                break;
            case GameManager.LineType.Diagonal:
                if (e.line.lineIndex == 0)
                {
                    eulerZ = 45;
                }
                else
                {
                    eulerZ = -45;
                }
                    break;
            default:
                break;
        }

        Vector2 pos2D = GetGridWorldPosition(e.line.centerGridPosition.x, e.line.centerGridPosition.y);
        Vector3 pos3D = new Vector3(pos2D.x, pos2D.y, lineCompletePrefab.position.z);
        Transform line = Instantiate(lineCompletePrefab, pos3D, Quaternion.Euler(0, 0, eulerZ));
        line.GetComponent<NetworkObject>().Spawn(true);
    }


    [Rpc(SendTo.Server)]
    void SpawnObjectRpc(int x,  int y, PlayerType playerType)
    {
        Transform prefab;
        switch (playerType)
        {
            case PlayerType.Cross:
                prefab = crossPrefab;
                break;
            case PlayerType.Circle:
                prefab = circlePrefab;
                break;
            default:
                prefab = crossPrefab;
                break;
        }

        var pos = GetGridWorldPosition(x, y);
        Transform spawned = Instantiate(prefab, pos, Quaternion.identity);
        spawned.GetComponent<NetworkObject>().Spawn(true);
    }

    Vector2 GetGridWorldPosition(int x,  int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
