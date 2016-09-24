using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AIBee), true)]
[CanEditMultipleObjects]
public class AIBeeRegionEditor : Editor {
    public void OnSceneGUI() {
        baseGUI();

        AIBee aiBee = (target as AIBee);

        if (!aiBee.goAwayParameters.endMoving) return;

        Region walk = aiBee.flyingParameters.walkArea;
        Vector3 parentPosition = aiBee.transform.position;
        bool showWalk = !aiBee.flyingParameters.usePath;


        switch (Tools.current) {
            case Tool.Scale:
                EditorGUI.BeginChangeCheck();
                float scaleHandle = HandleUtility.GetHandleSize(walk.origin);

                Vector3 scale = Handles.ScaleHandle(walk.size, walk.origin + parentPosition,
                                                    Quaternion.identity, scaleHandle);
                
                if (EditorGUI.EndChangeCheck()) {
                    if (showWalk)
                        walk.size = scale;
                }
                break;

            case Tool.Move:
                EditorGUI.BeginChangeCheck();
                Vector3 move = Handles.PositionHandle(walk.origin + parentPosition,
                                                            Quaternion.identity);

                
                if (EditorGUI.EndChangeCheck()) {
                    if (showWalk)
                        walk.origin = move - parentPosition;
                }
                break;
        }

        if (GUI.changed) {
            EditorUtility.SetDirty(aiBee);
        }

    }

    private void baseGUI() {
        AIBase aiBase = (target as AIBase);
        Region region = aiBase.triggerArea;
        Region walk = aiBase.walkingParameters.walkArea;
        Vector3 parentPosition = aiBase.transform.position;
        bool showWalk = !aiBase.walkingParameters.usePath && aiBase.commonParameters.walking;


        switch (Tools.current) {
            case Tool.Scale:
                EditorGUI.BeginChangeCheck();
                float scaleHandle = HandleUtility.GetHandleSize(region.origin);
                Vector3 scale = Handles.ScaleHandle(region.size, region.origin + parentPosition,
                                                    Quaternion.identity, scaleHandle);

                scaleHandle = HandleUtility.GetHandleSize(walk.origin);

                Vector3 scale2 = Vector3.zero;
                if (showWalk) {
                    scale2 = Handles.ScaleHandle(walk.size, walk.origin + parentPosition,
                                                    Quaternion.identity, scaleHandle);
                }
                if (EditorGUI.EndChangeCheck()) {
                    region.size = scale;
                    if (showWalk)
                        walk.size = scale2;
                }
                break;

            case Tool.Move:
                EditorGUI.BeginChangeCheck();
                Vector3 move = Handles.PositionHandle(region.origin + parentPosition,
                                                            Quaternion.identity);


                Vector3 move2 = Vector3.zero;
                if (showWalk)
                    move2 = Handles.PositionHandle(walk.origin + parentPosition,
                                                            Quaternion.identity);
                if (EditorGUI.EndChangeCheck()) {
                    region.origin = move - parentPosition;
                    if (showWalk)
                        walk.origin = move2 - parentPosition;
                }
                break;
        }
    }
}