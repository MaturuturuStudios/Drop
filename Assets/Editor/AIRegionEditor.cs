using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AIBase), true)]
[CanEditMultipleObjects]
public class RegionEditor : Editor {
    public void OnSceneGUI() {
        AIBase aiBase = (target as AIBase);
        Region region = aiBase.triggerArea;
        Region walk = aiBase.walkingParameters.walkArea;
        Vector3 parentPosition = aiBase.transform.position;


        switch (Tools.current) {
            case Tool.Scale:
                EditorGUI.BeginChangeCheck();
                float scaleHandle = HandleUtility.GetHandleSize(region.origin);
                Vector3 scale = Handles.ScaleHandle(region.size, region.origin+ parentPosition,
                                                    Quaternion.identity, scaleHandle);

                scaleHandle = HandleUtility.GetHandleSize(walk.origin);
                Vector3 scale2 = Handles.ScaleHandle(walk.size, walk.origin + parentPosition,
                                                    Quaternion.identity, scaleHandle);
                if (EditorGUI.EndChangeCheck()) {
                    region.size = scale;
                    walk.size = scale2;
                }
                break;

            case Tool.Move:
                EditorGUI.BeginChangeCheck();
                Vector3 move = Handles.PositionHandle(region.origin+parentPosition,
                                                            Quaternion.identity);
                
                Vector3 move2 = Handles.PositionHandle(walk.origin + parentPosition,
                                                            Quaternion.identity);
                if (EditorGUI.EndChangeCheck()) {
                    region.origin = move - parentPosition;
                    walk.origin = move2 - parentPosition;
                }
                break;

                //case Tool.Rotate:
                //    EditorGUI.BeginChangeCheck();
                //    Quaternion rotation = Handles.RotationHandle(region.rotation, region.getPositionWorld());
                //    if (EditorGUI.EndChangeCheck()) {
                //        region.rotation = rotation;
                //    }
                //    break;
        }

    }
}