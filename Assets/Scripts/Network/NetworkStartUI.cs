using Unity.Netcode;
using UnityEngine;
using TMPro;

public class NetworkStartUI : MonoBehaviour
{
    [SerializeField] GameObject networkPanel;
    [SerializeField] TestRelay relayManager;
    [SerializeField] TMP_InputField joinCodeInputField;

    private void OnEnable()
    {
        relayManager.OnRelayCreated += OnRelayCreated;
    }    

    private void OnDisable()
    {
        relayManager.OnRelayCreated -= OnRelayCreated;
    }

    private void OnRelayCreated(string joinCode)
    {
        joinCodeInputField.text = joinCode;
    }

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

    public void CreateRelay()
    {
        relayManager.CreateRelay();
    }

    public void JoinRelay()
    {
        relayManager.JoinRelay(joinCodeInputField.text);
    }
}
