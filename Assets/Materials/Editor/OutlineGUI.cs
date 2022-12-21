using UnityEditor;

public class OutlineGUI : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);

        // set to integer
        MaterialProperty prop = ShaderGUI.FindProperty("_Thickness", properties);
        prop.floatValue = (int)prop.floatValue;


    }
}