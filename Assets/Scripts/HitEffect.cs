using System.Collections.Generic;
using UnityEngine;

public class HitEffect
{
    public GameObject gameObject;

    Material _hitMaterial;

    float alpha = 0f;

    public HitEffect(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    Dictionary<Renderer, Material[]> _originalMaterials = new Dictionary<Renderer, Material[]>();

    public void Initialize()
    {
        // Add a red material with 0 alpha
        _hitMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        _hitMaterial.color = new Color(1, 0, 0, 0);

        // Set surface type to transparent
        _hitMaterial.SetFloat("_Surface", 1f);

        // Set transparency
        _hitMaterial.SetFloat("_Mode", 3);
        _hitMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _hitMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _hitMaterial.renderQueue = 3000;

        _hitMaterial.SetInt("_ZWrite", 0);
        _hitMaterial.DisableKeyword("_ALPHATEST_ON");
        _hitMaterial.EnableKeyword("_ALPHABLEND_ON");
        _hitMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        // Add to the materials list of every single renderer
        foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>())
        {
            _originalMaterials.Add(renderer, renderer.materials);
        }
    }

    public void OnDamage()
    {
        alpha = 1f;
    }

    public void Update()
    {
        foreach (var renderer in _originalMaterials.Keys)
        {
            UpdateMaterials(renderer);
        }

        if (alpha > 0)
            alpha -= Time.deltaTime;

        _hitMaterial.color = new Color(1, 0, 0, alpha);
    }

    void UpdateMaterials(Renderer renderer)
    {
        // Get original materials
        var materials = _originalMaterials[renderer];

        var newMaterials = new List<Material>();
        newMaterials.AddRange(materials);
        newMaterials.Add(_hitMaterial);

        renderer.materials = newMaterials.ToArray();
    }
}
