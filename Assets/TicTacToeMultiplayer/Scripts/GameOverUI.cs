using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] TMP_Text restultText;
    [SerializeField] Color winColor = Color.green;
    [SerializeField] Color loseColor = Color.red;
    [SerializeField] GameObject panelGameOverUI;
    [SerializeField] Button rematchButton;

    private void Awake()
    {
        rematchButton.onClick.AddListener(() => GameManager.Instance.RematchRpc());
    }

    private void Start()
    {
        GameManager.Instance.OnGameWin += OnGameWin;
        GameManager.Instance.OnRematch += OnRematch;

        Hide();
    }

    private void OnRematch(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameWin -= OnGameWin;
        GameManager.Instance.OnRematch -= OnRematch;
    }

    private void OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (e.winPlayerType == GameManager.Instance.LocalPlayerType)
        {
            restultText.text = "YOU WIN!";
            restultText.color = winColor;
        }
        else
        {
            restultText.text = "YOU LOSE";
            restultText.color = loseColor;
        }
        Show();
    }

    void Show()
    {
        print("Show GameOver UI");
        panelGameOverUI.SetActive(true);
    }

    void Hide()
    {
        panelGameOverUI.SetActive(false);
    }

}
