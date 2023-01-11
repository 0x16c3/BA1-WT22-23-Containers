using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatSystem
{
    public static Renderer GetRenderer(GameObject obj)
    {
        // Get renderer
        Renderer renderer = obj.GetComponent<Renderer>();

        // If null, check each child
        if (renderer == null)
        {
            foreach (Transform child in obj.transform)
            {
                renderer = child.GetComponent<Renderer>();

                if (renderer != null)
                    break;
            }
        }

        return renderer;
    }

    public static bool SameMaterial(Material mat1, Material mat2)
    {
        if (mat1 == mat2)
            return true;

        // Check name of the material as well, in case its instanced
        if (mat1.name == mat2.name || mat1.name == mat2.name + " (Instance)")
            return true;

        return false;
    }

    public static bool HasMaterial(GameObject obj, Material mat)
    {
        if (GetRenderer(obj) == null)
            return false;

        Material[] materials = GetRenderer(obj).materials;

        foreach (Material material in materials)
        {
            if (material == mat)
                return true;

            if (SameMaterial(material, mat))
                return true;
        }

        return false;
    }

    public static void AddMaterial(GameObject obj, Material mat)
    {
        if (GetRenderer(obj) == null)
            return;

        Material[] materials = GetRenderer(obj).materials;

        if (HasMaterial(obj, mat))
            return;

        // Add outline material to the object
        Material[] newMaterials = new Material[materials.Length + 1];
        materials.CopyTo(newMaterials, 0);

        newMaterials[materials.Length] = mat;
        GetRenderer(obj).materials = newMaterials;
    }

    public static void RemoveMaterial(GameObject obj, Material mat)
    {
        if (GetRenderer(obj) == null)
            return;

        Material[] materials = GetRenderer(obj).materials;
        var newMaterials = new List<Material>();

        for (int i = 0; i < materials.Length; i++)
        {
            if (SameMaterial(materials[i], mat))
                continue;

            newMaterials.Add(materials[i]);
        }

        GetRenderer(obj).materials = newMaterials.ToArray();
    }
}
