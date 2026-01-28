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
}
