using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    private const float GRID_SIZE = 4f;

    [SerializeField] Transform crossPrefab;
    [SerializeField] Transform circlePrefab;   
    

    private void Start()
    {
        GameManager.Instance.OnClickedOnGridPosition += OnClickedOnGridPosition;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnClickedOnGridPosition -= OnClickedOnGridPosition;
    }

    private void OnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionEventArgs e)
    {
        SpawnObjectRpc(e.x, e.y, e.playerType);
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
