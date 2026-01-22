using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] GameObject crossArrow;
    [SerializeField] GameObject circleArrow;
    [SerializeField] GameObject crossYouText;
    [SerializeField] GameObject circleYouText;


    private void Awake()
    {
        crossArrow.SetActive(false);
        circleArrow.SetActive(false);
        crossYouText.SetActive(false);
        circleYouText.SetActive(false);
    }

    private void Start()
    {
        GameManager.Instance.OnGameStarted += OnGameStarted;
        GameManager.Instance.OnCurrentPlayablePlayerTypeChanged += OnCurrentPlayablePlayerTypeChanged;
    }

    private void OnCurrentPlayablePlayerTypeChanged(object sender, System.EventArgs e)
    {
        UpdateCurrentArrow();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStarted -= OnGameStarted;
    }

    private void OnGameStarted(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.LocalPlayerType == PlayerType.Cross)
            crossYouText.SetActive(true);
        else
            circleYouText.SetActive(true);

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
