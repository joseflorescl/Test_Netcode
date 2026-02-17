using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : NetworkBehaviour
{
    [SerializeField] int nCounter = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            int n1 = nCounter;
            Interact();
            int n2 = nCounter;

            if (n1 == n2)
            {
                print($"NO: la actualización de nCounter NO fue inmediata."); // Esto pasa si llamamos a IncrementCounterServerRpc(); desde Interact()
            }
            else 
            {
                print($"SI: la actualización de nCounter SI fue inmediata.");
            }
        }
    }

    void Interact()
    {
        //IncrementCounterServerRpc();
        IncrementCounterClientRpc();
    }

    [Rpc(SendTo.Server)]
    void IncrementCounterServerRpc()
    {
        IncrementCounterClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void IncrementCounterClientRpc() 
    {
        nCounter++;
    }
    


}
