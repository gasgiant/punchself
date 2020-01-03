using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoundLibraryAsset))]
public class SoundLibraryAssetEditor : Editor
{
    private static GUIContent
        duplicateButtonContent = new GUIContent("+", "duplicate"),
        deleteButtonContent = new GUIContent("-", "delete");

    private GUILayoutOption miniButtonWidth = GUILayout.Width(20f);


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SoundLibraryAsset soundLibrary = (SoundLibraryAsset)serializedObject.targetObject;

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontStyle = FontStyle.Bold;
        if (soundLibrary.editMode)
            GUI.color = new Color(1f, 0.8f, 0.8f);

        GUILayoutOption editModeButtonWidth = GUILayout.Width(100f);

        if (GUILayout.Button("Edit mode", editModeButtonWidth))
        {
            soundLibrary.editMode = !soundLibrary.editMode;
        }
        if (soundLibrary.editMode)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("soundGroups").FindPropertyRelative("Array.size"));

        EditorGUILayout.Space();
        if (soundLibrary.soundGroups != null && soundLibrary.soundGroups.Count > 0)
        {
            for (int i = 0; i < serializedObject.FindProperty("soundGroups").arraySize; i++)
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.BeginHorizontal();
                if (i < soundLibrary.soundGroups.Count && i < serializedObject.FindProperty("soundGroups").arraySize)
                {
                    if (soundLibrary.editMode)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("soundGroups").GetArrayElementAtIndex(i).FindPropertyRelative("groupID"));
                    else
                        EditorGUILayout.LabelField(soundLibrary.soundGroups[i].groupID, labelStyle);
                }

                if (soundLibrary.editMode)
                    ShowButtons(serializedObject.FindProperty("soundGroups"), i);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel += 1;
                if (i < soundLibrary.soundGroups.Count && i < serializedObject.FindProperty("soundGroups").arraySize)
                    ShowSoundGroup(soundLibrary.soundGroups[i], serializedObject.FindProperty("soundGroups").GetArrayElementAtIndex(i));

                if (GUILayout.Button("Set defaults", GUILayout.Width(100f)))
                {
                    if (i < soundLibrary.soundGroups.Count)
                    {
                        soundLibrary.soundGroups[i].volume = 1;
                        soundLibrary.soundGroups[i].deadTime = 0;
                        soundLibrary.soundGroups[i].randomize = false;
                        soundLibrary.soundGroups[i].pitchRndValue = 0.1f;
                        soundLibrary.soundGroups[i].volumeRndValue = 0.1f;
                        soundLibrary.soundGroups[i].interruptible = false;
                        soundLibrary.soundGroups[i].maxSoursesCount = 5;
                        soundLibrary.soundGroups[i].fadeOutTime = 0.2f;
                        soundLibrary.soundGroups[i].loop = false;
                        soundLibrary.soundGroups[i].maxLoopCount = 10;
                        soundLibrary.soundGroups[i].loopMode = SoundLoopMode.Simple;
                        soundLibrary.soundGroups[i].fadeStartTime = 0;
                        soundLibrary.soundGroups[i].fadeDuration = 0.2f;
                    }
                }
                EditorGUI.indentLevel -= 1;

                EditorGUILayout.Space();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void ShowSoundGroup(SoundGroup soundGroup, SerializedProperty property)
    {
        /*
        EditorGUILayout.PropertyField(property.FindPropertyRelative("sounds"));
        if (property.FindPropertyRelative("sounds").isExpanded)
        {
            EditorGUI.indentLevel += 1;
            if (property.FindPropertyRelative("sounds").arraySize == 0)
            {
                EditorGUILayout.BeginHorizontal();
                ShowButtons(property.FindPropertyRelative("sounds"), 0);
                EditorGUILayout.EndHorizontal();
            }
            for (int j = 0; j < property.FindPropertyRelative("sounds").arraySize; j++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(property.FindPropertyRelative("sounds").GetArrayElementAtIndex(j), GUIContent.none);
                ShowButtons(property.FindPropertyRelative("sounds"), j);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel -= 1;
        }
        */
        EditorList.Show(property.FindPropertyRelative("sounds"), EditorListOption.NoSize);

        EditorGUILayout.Slider(property.FindPropertyRelative("volume"), 0, 1);
        EditorGUILayout.PropertyField(property.FindPropertyRelative("deadTime"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("randomize"));

        if (soundGroup.randomize)
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Slider(property.FindPropertyRelative("pitchRndValue"), 0, 1);
            EditorGUILayout.Slider(property.FindPropertyRelative("volumeRndValue"), 0, 1);
            EditorGUI.indentLevel -= 1;
        }

        EditorGUILayout.PropertyField(property.FindPropertyRelative("interruptible"));
        if (soundGroup.interruptible)
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(property.FindPropertyRelative("maxSoursesCount"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("fadeOutTime"));
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.PropertyField(property.FindPropertyRelative("loop"));
            if (soundGroup.loop)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(property.FindPropertyRelative("loopMode"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("maxLoopCount"));
                if (soundGroup.loopMode == SoundLoopMode.Crossfade)
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("fadeStartTime"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("fadeDuration"));
                }
                EditorGUI.indentLevel -= 1;
            }
            
        }
        
    }

    private void ShowButtons(SerializedProperty list, int index)
    {
        if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
        {
            list.InsertArrayElementAtIndex(index);
        }
        if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
        {
            int oldSize = list.arraySize;

            list.DeleteArrayElementAtIndex(index);
            if (list.arraySize == oldSize)
            {
                list.DeleteArrayElementAtIndex(index);
            }
        }
    }
}
