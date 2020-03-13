using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var map = (Map) target;
        
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Generate Map")) map.GenerateMap();
    }
}