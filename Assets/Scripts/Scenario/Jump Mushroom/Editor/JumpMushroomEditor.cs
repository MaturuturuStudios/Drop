using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(JumpMushroom))]
[CanEditMultipleObjects]
public class JumpMushroomEditor : Editor {

   
    public override  void OnInspectorGUI(){


        var myScript = target as JumpMushroom;

        EditorUtility.SetDirty(target);

        //myScript.length = EditorGUILayout.FloatField("Collider Length", myScript.length);
        //myScript.width = EditorGUILayout.FloatField("Collider Width", myScript.width);

        myScript.minheight = EditorGUILayout.FloatField("Min Height",myScript.minheight);
        myScript.maxheight = EditorGUILayout.FloatField("Max Height", myScript.maxheight);
       
        myScript.Jumpforce = EditorGUILayout.FloatField("Jump Force", myScript.Jumpforce);
        myScript.KeepVerticalSpeed = GUILayout.Toggle(myScript.KeepVerticalSpeed, "Keep Perpendicular Speed ");

        myScript.lostcontrol = GUILayout.Toggle(myScript.lostcontrol, "Lost Control");

       

        if (myScript.KeepVerticalSpeed)
            myScript.KeepVerticalSpeed = true;
        if (myScript.KeepVerticalSpeed==false)
            myScript.KeepVerticalSpeed = false;


        if (myScript.lostcontrol) {
            myScript.temporaly = GUILayout.Toggle(myScript.temporaly, "Lost Control Temporaly");
            
        }
        if (myScript.temporaly) {

            myScript.time = EditorGUILayout.FloatField("Time", myScript.time);

        }
        if (myScript.temporaly==false)
        {

            myScript.time = 0.0f ;

        }
    }
}

