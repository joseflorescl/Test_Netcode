using UnityEngine;

public class ConfigVisual : MonoBehaviour
{
    [SerializeField] protected Material[] availableMaterials;
    [SerializeField] protected float minScale = 0.75f;
    [SerializeField] protected float maxScale = 1.2f;
    [SerializeField] protected GameObject model;
    [SerializeField] bool assignRandomMaterialOnStart = true;


    public Material[] AvailableMaterials => availableMaterials;
    public bool AssignRandomMaterialOnStart { get => assignRandomMaterialOnStart; set => assignRandomMaterialOnStart = value; }

    private void Awake()
    {
        Debug.Log("Awake. ConfigVisual", gameObject);
    }    

    protected virtual void Start()
    {
        print("ConfigVisual.Start");

        if (assignRandomMaterialOnStart)
        {
            SetRandomMaterial();
        }
        
        SetRandomScale();
    }

    protected void SetRandomMaterial()
    {
        print("SetRandomMaterial");
        int idx = Random.Range(0, availableMaterials.Length);
        SetMaterialFromIndex(idx);
    }

    public void SetMaterialFromIndex(int idx)
    {
        print($"SetMaterialFromIndex. idx: {idx}");
        var material = availableMaterials[idx];
        Renderer rend = model.GetComponent<Renderer>();
        rend.material = material;
    }

    protected void SetRandomScale()
    {
        float scale = Random.Range(minScale, maxScale);
        Vector3 currentScale = model.transform.localScale;
        Vector3 newScale = currentScale * scale;
        model.transform.localScale = newScale;

        Debug.Log($"SetRandomScale: {scale}", gameObject);
    }

}
