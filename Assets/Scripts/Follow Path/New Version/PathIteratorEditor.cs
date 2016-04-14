using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PathIterator))]
public class LevelScriptEditor : Editor {
    public override void OnInspectorGUI() {

        DrawDefaultInspector();

        PathIterator myTarget = (PathIterator)target;

        myTarget.stepByStep = EditorGUILayout.Toggle("Step By Step", myTarget.stepByStep);
        if (myTarget.stepByStep) {
            myTarget.numberOfSteps = EditorGUILayout.IntSlider("Number Of Steps To Move", myTarget.numberOfSteps, 1, 100);
            myTarget.directly = EditorGUILayout.Toggle("Move directly", myTarget.directly);
        }
    }
}