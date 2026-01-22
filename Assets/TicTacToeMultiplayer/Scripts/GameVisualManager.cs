using UnityEngine;

public class GameVisualManager : MonoBehaviour
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
        var pos = GetGridWorldPosition(e.x, e.y);
        Instantiate(crossPrefab, pos, Quaternion.identity);
    }

    Vector2 GetGridWorldPosition(int x,  int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
