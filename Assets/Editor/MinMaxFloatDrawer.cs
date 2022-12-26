using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxFloat))]
[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class MinMaxFloatDrawer : PropertyDrawer
{
    private static MinMaxRangeAttribute m_DefaultRange = new MinMaxRangeAttribute(0, 1);
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MinMaxRangeAttribute limits = attribute as MinMaxRangeAttribute;
        if (limits == null)
            limits = m_DefaultRange;
        var minProp = property.FindPropertyRelative("min");
        var maxProp = property.FindPropertyRelative("max");
        float min = minProp.floatValue;
        float max = maxProp.floatValue;
        label.text += "(" + min.ToString("0.00") + ", " + max.ToString("0.00") + ")";
        EditorGUI.BeginChangeCheck();
        EditorGUI.MinMaxSlider(position, label, ref min, ref max, limits.min, limits.max);
        if (EditorGUI.EndChangeCheck() || !limits.IsInRange(min) || !limits.IsInRange(max))
        {
            minProp.floatValue = Mathf.Clamp(min, limits.min, limits.max);
            maxProp.floatValue = Mathf.Clamp(max, limits.min, limits.max);
        }
    }
}