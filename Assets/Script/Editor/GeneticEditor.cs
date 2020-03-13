using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Genetic))]
public class GeneticEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var genetic = (Genetic) target;
        
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Create Monster")) genetic.CreateMonster();
    }
}
