using UnityEngine;

public class GridPosition : MonoBehaviour
{
    [SerializeField] int x;
    [SerializeField] int y;

    private void OnMouseDown()
    {
        if (GameManager.Instance.CurrentPlayablePlayerType == PlayerType.None)
        {
            print("El juego no ha comenzado todavía");
            return;
        }
        GameManager.Instance.ClickedOnGridPositionRpc(x, y, GameManager.Instance.LocalPlayerType);
    }
}
