using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AIBase), true)]
[CanEditMultipleObjects]
public class RegionEditor : Editor {
    public void OnSceneGUI() {
        AIBase aiBase = (target as AIBase);
        AIBase.Region region = aiBase.triggerArea;
        Vector3 parentPosition = aiBase.transform.position;


        switch (Tools.current) {
            case Tool.Scale:
                EditorGUI.BeginChangeCheck();
                float scaleHandle = HandleUtility.GetHandleSize(region.origin);
                Vector3 scale = Handles.ScaleHandle(region.size, region.origin+ parentPosition,
                                                    Quaternion.identity, scaleHandle);
                if (EditorGUI.EndChangeCheck()) {
                    region.size = scale;
                }
                break;

            case Tool.Move:
                EditorGUI.BeginChangeCheck();
                Vector3 move = Handles.PositionHandle(region.origin+parentPosition,
                                                            Quaternion.identity);
                if (EditorGUI.EndChangeCheck()) {
                    region.origin = move - parentPosition;
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