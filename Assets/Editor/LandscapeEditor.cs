using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LandscapeRoot))]
public class TestOnInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Generate"))
        {
            LandscapeRoot landscape = (LandscapeRoot)target;
            landscape.Generate();
        }
    }
}