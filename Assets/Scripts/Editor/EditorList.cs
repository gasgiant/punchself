using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class EditorList
{
    private static GUIContent
        moveButtonContent = new GUIContent("\u21b4", "move down"),
        duplicateButtonContent = new GUIContent("+", "duplicate"),
        deleteButtonContent = new GUIContent("-", "delete");

    public static void Show(SerializedProperty list, EditorListOption options = EditorListOption.Default, string elementsLabel = "")
    {
        bool
            showListLabel = (options & EditorListOption.ListLabel) != 0,
            showListSize = (options & EditorListOption.ListSize) != 0;
            

        if (showListLabel)
        {
            EditorGUILayout.PropertyField(list);
            EditorGUI.indentLevel += 1;
            if (showListSize && list.isExpanded)
            {
                EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
            }

        }

        if (!showListLabel || list.isExpanded)
        {
            if (showListSize & !showListLabel)
            {
                EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"), GUIContent.none);
            }

            ShowElements(list, options, elementsLabel);
        }

        if (showListLabel)
        {
            EditorGUI.indentLevel -= 1;
        }
    }

    private static void ShowElements(SerializedProperty list, EditorListOption options, string elementsLabel)
    {
        bool
            showElementLabels = (options & EditorListOption.ElementLabels) != 0,
            showButtons = (options & EditorListOption.Buttons) != 0,
            showChildren = (options & EditorListOption.Children) != 0,
            moveButton = (options & EditorListOption.MoveButton) != 0,
            showListSize = (options & EditorListOption.ListSize) != 0;

        if (showButtons && !showListSize && list.arraySize < 1)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 15);
            if (GUILayout.Button(duplicateButtonContent, GUILayout.Width(60f)))
            {
                list.InsertArrayElementAtIndex(0);
            }
            EditorGUILayout.EndHorizontal();
        }

        for (int i = 0; i < list.arraySize; i++)
        {
            if (showButtons)
            {
                EditorGUILayout.BeginHorizontal();
            }
            if (showElementLabels)
            {
                if (elementsLabel == "")
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), showChildren);
                else
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent(elementsLabel + " " + i.ToString()), showChildren);
            }
            else
            {
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none, showChildren);
            }
            if (showButtons)
            {
                ShowButtons(list, i, moveButton);
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

    private static void ShowButtons(SerializedProperty list, int index, bool showMoveButtton)
    {
        if (showMoveButtton)
        {
            if (GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
            {
                list.MoveArrayElement(index, index + 1);
            }
            if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonMid, miniButtonWidth))
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
        else
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
}

[Flags]
public enum EditorListOption
{
    None = 0,
    ListSize = 1,
    ListLabel = 2,
    ElementLabels = 4,
    Buttons = 8,
    Children = 16,
    MoveButton = 32,
    Default = ListSize | ListLabel | ElementLabels | Children,
    All = Default | Buttons | MoveButton,
    ButtonsAndSize = ListSize | Buttons | Children,
    ButtonsSizeLabel = ListSize | Buttons | ListLabel | Children,
    NoListLabel = ListSize | Buttons | ElementLabels | Children,
    ButtonsLabelsChildren = Buttons | ElementLabels | Children,
    SizeLabels = ListSize | ElementLabels | Children,
    NoSize = ListLabel | Buttons | ElementLabels | Children,
}
