using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] Button starHost;
    [SerializeField] Button starClient;

    private void Awake()
    {
        starHost.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            Hide();
        });
        starClient.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            Hide();
        });

    }

    void Hide()
    {
        gameObject.SetActive(false);
    }


}
