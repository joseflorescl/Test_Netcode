using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource SFXAudioSource;

    [SerializeField] AudioClip placeSFX;
    [SerializeField] AudioClip winSFX;
    [SerializeField] AudioClip loseSFX;

    private void Start()
    {
        GameManager.Instance.OnPlacedObject += OnPlacedObject;
        GameManager.Instance.OnGameWin += OnGameWin;        
    }    

    private void OnDisable()
    {
        GameManager.Instance.OnPlacedObject -= OnPlacedObject;
        GameManager.Instance.OnGameWin -= OnGameWin;
    }

    private void OnPlacedObject(object sender, System.EventArgs e)
    {
        print($"SFX: {placeSFX.name}");
        SFXAudioSource.PlayOneShot(placeSFX);
    }

    private void OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {

        if (GameManager.Instance.LocalPlayerType == e.winPlayerType)
        {
            SFXAudioSource.PlayOneShot(winSFX);
        }
        else
        {
            SFXAudioSource.PlayOneShot(loseSFX);
        }

        
    }
}
