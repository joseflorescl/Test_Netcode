using Unity.Netcode;
using UnityEngine;

public class ConfigVisualState : NetworkBehaviour
{
    public NetworkVariable<int> MaterialIndex = new();


    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        // TODO: me falta acceder a la data que ya viene en ConfigVisual
        //MaterialIndex.Value = Random.Range(0, availableMaterials.Length);
    }
}
