using UnityEngine;

public class ConfigVisual : MonoBehaviour
{
    [SerializeField] Material[] availableMaterials;
    [SerializeField] float minScale = 0.75f;
    [SerializeField] float maxScale = 1.2f;
    [SerializeField] GameObject model;

    private void Awake()
    {
        int idx = Random.Range(0, availableMaterials.Length);
        var material = availableMaterials[idx];
        Renderer rend = model.GetComponent<Renderer>();
        rend.material = material;

        float scale = Random.Range(minScale, maxScale);
        Vector3 currentScale = model.transform.localScale;
        Vector3 newScale = currentScale * scale;
        model.transform.localScale = newScale;
    }

}
