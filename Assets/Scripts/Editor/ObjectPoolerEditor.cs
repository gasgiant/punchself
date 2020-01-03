using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectPooler))]
public class ObjectPoolerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorList.Show(serializedObject.FindProperty("pools"), EditorListOption.NoListLabel);

        serializedObject.ApplyModifiedProperties();

    }
}
