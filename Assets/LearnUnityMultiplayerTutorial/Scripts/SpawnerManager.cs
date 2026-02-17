using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnerManager : NetworkBehaviour
{
    [SerializeField] Transform objectPrefab;
    [SerializeField] float minX = -8f;
    [SerializeField] float maxX = 8f;
    [SerializeField] float minY = -4f;
    [SerializeField] float maxY = 6f;

    List<NetworkObjectReference> netObjSpawnedList = new(); // Solo el server maneja esta var

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SpawnObjectServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            TestClientRpc(Random.Range(1, 11));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            // El destroy SOLO se puede hacer en el server
            DestroyOneNetObjectServerRpc();
        }
    }


    [Rpc(SendTo.Server)]
    void SpawnObjectServerRpc()
    {
        // Si el cliente llama esta función se cae: solo el server puede hacer Spawn.

        // setear posición random
        Transform tr = Instantiate(objectPrefab);
        float xPos = Random.Range(minX, maxX);
        float yPos = Random.Range(minY, maxY);
        tr.position = new Vector3(xPos, yPos, tr.position.z);        

        // spawn
        var netObj = tr.GetComponent<NetworkObject>();
        netObj.Spawn(true);

        // Broadcast: cambiarle el color
        SetColorToObjectSpawnedClientRpc(netObj, Random.ColorHSV());

        // Solo el server lo agrega a su lista de objetos spawneados
        AddNewNetObjSpawned(netObj);

    }

    [Rpc(SendTo.ClientsAndHost)]
    void SetColorToObjectSpawnedClientRpc(NetworkObjectReference netReference, Color color)
    {
        netReference.TryGet(out NetworkObject netObj);
        Transform tr = netObj.GetComponent<Transform>();
        ChangeColor(tr, color);
    }

    void ChangeColor(Transform tr, Color color)
    {
        var rend = tr.GetComponentInChildren<Renderer>();
        rend.material.color = color;
    }

    [Rpc(SendTo.ClientsAndHost)]
    void TestClientRpc(int nTest)
    {
        print($"nTest: {nTest}");
    }
    
    void AddNewNetObjSpawned(NetworkObjectReference netReference)
    {
        netObjSpawnedList.Add(netReference);
    }

    [Rpc(SendTo.Server)]
    void DestroyOneNetObjectServerRpc()
    {
        if (netObjSpawnedList == null || netObjSpawnedList.Count == 0)
            return;

        var netObj = netObjSpawnedList[netObjSpawnedList.Count - 1];
        netObjSpawnedList.Remove(netObj);
        netObj.TryGet(out NetworkObject obj);
        Destroy(obj.gameObject);
    }
}
