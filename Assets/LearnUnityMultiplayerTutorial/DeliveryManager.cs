using UnityEngine;
using Unity.Netcode;
using TMPro;

public class DeliveryManager : NetworkBehaviour
{
    public static event System.Action OnMyEvent;

    [SerializeField] TMP_Text recipeCountText;

    int recipeCount;

    private void Awake()
    {
        recipeCount = 0;
        UpdateRecipeUI();
    }

    private void OnEnable()
    {
        DeliveryManager.OnMyEvent -= OnMyEventHandler;
        DeliveryManager.OnMyEvent += OnMyEventHandler;
    }

    private void OnDisable()
    {
        DeliveryManager.OnMyEvent -= OnMyEventHandler;
    }

    private void OnMyEventHandler()
    {
        print("OnMyEventHandler");
    }

    void UpdateRecipeUI()
    {
        recipeCountText.text = recipeCount.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            DeliverRecipe();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnMyEvent?.Invoke();
        }
    }
    public void DeliverRecipe()
    {
        // Se valida la receta: se debe comunicar a todos los clientes:
        DeliverCorrectRecipeServerRpc();
    }

    [Rpc(SendTo.Server)]
    void DeliverCorrectRecipeServerRpc()
    {
        // Broadcast a todos los clients que la receta es correcta
        DeliverCorrectRecipeClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void DeliverCorrectRecipeClientRpc()
    {
        // Aquí está el código original, que modifica vars y lanza eventos.
        recipeCount++;
        UpdateRecipeUI();
    }
}
