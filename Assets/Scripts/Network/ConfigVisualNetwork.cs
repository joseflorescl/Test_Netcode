using Unity.Netcode;
using UnityEngine;

public class ConfigVisualNetwork : NetworkAuthorityComponentDisabler<ConfigVisual>
{
    [SerializeField] bool ownerChoosesColorOnSpawn = true;

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

        if (ownerChoosesColorOnSpawn)
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
        else
        {
            if (IsServer)
            {
                int randomValue = Random.Range(0, configVisual.AvailableMaterials.Length);
                print($"El server elige el color para el player en el spawn: {randomValue}");
                MaterialIndex.Value = randomValue; // Esto gatillará el evento, y se hará el cambio de color.
            }
            else if (!IsOwner)
            {
                // En el cliente, el color YA fue seleccionado por el server
                print($"El cliente (que no es el owner): usa el valor actual de MaterialIndex seteado por el server: {MaterialIndex.Value}");
                configVisual.SetMaterialFromIndex(MaterialIndex.Value);
            }
            // Para el caso del player del owner, el server elegirá mi color, pero como es probable que el evento OnNetworkSpawn
            // primero se gatillen en el cliente y NO en el server, el valor de MaterialIndex.Value todavía no ha sido seteado
            // por el server: debemos esperar a que se gatille el evento.
        }

    }

    [ServerRpc]
    void CommitNetworkMaterialIndexServerRpc(int value)
    {
        MaterialIndex.Value = value;
    }

    void OnMaterialChanged(int oldIdx, int newIdx)
    {
        print($"Evento OnMaterialChanged. newIdx:{newIdx}. old: {oldIdx}");
        configVisual.SetMaterialFromIndex(newIdx);
    }
    
}
