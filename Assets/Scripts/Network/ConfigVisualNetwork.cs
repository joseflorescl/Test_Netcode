using Unity.Netcode;
using UnityEngine;

public class ConfigVisualNetwork : NetworkBehaviour
{
    NetworkVariable<int> MaterialIndex = new(value: -1);

    ConfigVisual configVisual;

    private void Awake()
    {
        configVisual = GetComponent<ConfigVisual>();
        configVisual.AssignRandomMaterialOnStart = false; // Porque se usará el evento OnMaterialChanged.
    }

    private void OnEnable()
    {
        MaterialIndex.OnValueChanged += OnMaterialChanged;
    }

    private void OnDisable()
    {
        MaterialIndex.OnValueChanged -= OnMaterialChanged;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            int randomValue = Random.Range(0, configVisual.AvailableMaterials.Length);
            print($"randomValue para MaterialIndex: {randomValue}");
            CommitNetworkMaterialIndexServerRpc(randomValue);
        }
        else if (!IsServer)
        {
            print($"NO soy el owner y NO soy server: uso el valor actual de MaterialIndex: {MaterialIndex.Value}");
            configVisual.SetMaterialFromIndex(MaterialIndex.Value);
        }        
    }

    [ServerRpc]
    void CommitNetworkMaterialIndexServerRpc(int value)
    {
        MaterialIndex.Value = value;
    }

    void OnMaterialChanged(int oldIdx, int newIdx)
    {
        print($"Evento OnMaterialChanged. newIdx:{newIdx}");
        configVisual.SetMaterialFromIndex(newIdx);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print($"Valor de index: {MaterialIndex.Value}");
        }
    }
}
