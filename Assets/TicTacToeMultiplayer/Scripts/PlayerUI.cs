using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] GameObject crossArrow;
    [SerializeField] GameObject circleArrow;
    [SerializeField] GameObject crossYouText;
    [SerializeField] GameObject circleYouText;
    [SerializeField] TMP_Text playerCrossScoreText;
    [SerializeField] TMP_Text playerCircleScoreText;


    private void Awake()
    {
        crossArrow.SetActive(false);
        circleArrow.SetActive(false);
        crossYouText.SetActive(false);
        circleYouText.SetActive(false);
        playerCrossScoreText.text = "";
        playerCircleScoreText.text = "";
    }

    private void Start()
    {
        GameManager.Instance.OnGameStarted += OnGameStarted;
        GameManager.Instance.OnCurrentPlayablePlayerTypeChanged += OnCurrentPlayablePlayerTypeChanged;
        GameManager.Instance.OnScoreChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStarted -= OnGameStarted;
        GameManager.Instance.OnCurrentPlayablePlayerTypeChanged -= OnCurrentPlayablePlayerTypeChanged;
        GameManager.Instance.OnScoreChanged -= OnScoreChanged;
    }    
    
    private void OnScoreChanged(object sender, System.EventArgs e)
    {
        var scores = GameManager.Instance.GetScores();
        playerCrossScoreText.text = scores.playerCrossScore.ToString();
        playerCircleScoreText.text = scores.playerCircleScore.ToString();
    }

    private void OnCurrentPlayablePlayerTypeChanged(object sender, System.EventArgs e)
    {
        UpdateCurrentArrow();
    }

    private void OnGameStarted(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.LocalPlayerType == PlayerType.Cross)
            crossYouText.SetActive(true);
        else
            circleYouText.SetActive(true);

        playerCrossScoreText.text = "0";
        playerCircleScoreText.text = "0";

        UpdateCurrentArrow();
    }

    void UpdateCurrentArrow()
    {
        bool cross;
        bool circle;
        if (GameManager.Instance.CurrentPlayablePlayerType == PlayerType.Cross)
        {
            cross = true;
            circle = false;
        }
        else 
        { 
            cross = false;
            circle = true;
        }
        crossArrow.SetActive(cross);
        circleArrow.SetActive(circle);
    }
}
