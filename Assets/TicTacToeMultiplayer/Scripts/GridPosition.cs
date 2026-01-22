using UnityEngine;

public class GridPosition : MonoBehaviour
{
    [SerializeField] int x;
    [SerializeField] int y;

    private void OnMouseDown()
    {
        GameManager.Instance.ClickedOnGridPosition(x, y);
    }
}
