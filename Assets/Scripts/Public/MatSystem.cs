using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatSystem
{
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
        if (obj.GetComponent<Renderer>() == null)
            return false;

        Material[] materials = obj.GetComponent<Renderer>().materials;

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
        if (obj.GetComponent<Renderer>() == null)
            return;

        Material[] materials = obj.GetComponent<Renderer>().materials;

        if (HasMaterial(obj, mat))
            return;

        // Add outline material to the object
        Material[] newMaterials = new Material[materials.Length + 1];
        materials.CopyTo(newMaterials, 0);

        newMaterials[materials.Length] = mat;
        obj.GetComponent<Renderer>().materials = newMaterials;
    }

    public static void RemoveMaterial(GameObject obj, Material mat)
    {
        if (obj.GetComponent<Renderer>() == null)
            return;

        Material[] materials = obj.GetComponent<Renderer>().materials;
        List<Material> newMaterials = new List<Material>();

        for (int i = 0; i < materials.Length; i++)
        {
            if (SameMaterial(materials[i], mat))
                continue;

            newMaterials.Add(materials[i]);
        }

        obj.GetComponent<Renderer>().materials = newMaterials.ToArray();
    }
}
