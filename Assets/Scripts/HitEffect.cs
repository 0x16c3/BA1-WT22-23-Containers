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

        // Set material name
        _hitMaterial.name = "HitMaterial";
    }

    public void OnDamage()
    {
        alpha = 1f;
    }

    public void Update()
    {
        // Update materials
        var renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            UpdateMaterials(renderer);
        }

        if (alpha > 0)
            alpha -= Time.deltaTime;

        _hitMaterial.color = new Color(1, 0, 0, alpha);
    }

    void UpdateMaterials(Renderer renderer)
    {
        // Get current materials
        var materials = new List<Material>(renderer.sharedMaterials);

        // Get outline component of the object
        var outline = renderer.gameObject.GetComponent<Outline>();
        if (outline != null && !outline.enabled)
        {
            // Remove outline materials
            materials.RemoveAll(m => m.name == "OutlineMask (Instance)" || m.name == "OutlineFill (Instance)");
        }

        var parentOutline = renderer.gameObject.GetComponentInChildren<Outline>();
        if (parentOutline != null && !parentOutline.enabled)
        {
            // Remove outline materials
            materials.RemoveAll(m => m.name == "OutlineMask (Instance)" || m.name == "OutlineFill (Instance)");
        }

        // Add hit material if not already added, check name "HitMaterial"
        var existingMaterial = materials.Find(m => m.name == _hitMaterial.name || m.name == $"{_hitMaterial.name} (Instance)");
        if (existingMaterial == null)
            materials.Add(_hitMaterial);
        else
            existingMaterial.color = _hitMaterial.color;

        // Apply materials
        renderer.materials = materials.ToArray();
    }
}
