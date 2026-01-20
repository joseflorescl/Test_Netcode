using Unity.Netcode;
using UnityEngine;

public class ConfigVisualNetwork : NetworkAuthorityComponentDisabler<ConfigVisual>
{
    NetworkVariable<int> MaterialIndex = new(value: -1);

    ConfigVisual configVisual;

    protected override void Awake()
    {
        base.Awake();
        configVisual = targetComponent;
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
        base.OnNetworkSpawn();

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
    
}
