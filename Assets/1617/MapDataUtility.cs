using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapData))]
public class MapDataUtility: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapData generator = (MapData)target;
        if (GUILayout.Button("Generate Random Map"))
        {
            generator.generateRandomMap();
        }

        if (GUILayout.Button("Add Robot"))
        {
            generator.addRobot();
        }
    }
}
