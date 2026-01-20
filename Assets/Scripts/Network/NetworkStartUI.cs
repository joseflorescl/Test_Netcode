using Unity.Netcode;
using UnityEngine;

public class NetworkStartUI : MonoBehaviour
{
    [SerializeField] GameObject networkPanel;

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Hide();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Hide();
    }

    void Hide()
    {
        networkPanel.SetActive(false);
    }
}
