using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(JumpMushroom))]
public class JumpMushroomEditor : Editor {

    override public void OnInspectorGUI(){

        /*
         public float minJump = 1;
    public float maxJump = 2;
    public float Jumpforce = 10;

    public bool KeepVerticalSpeed = true;*/

        var myScript = target as JumpMushroom;

        myScript.minheight = EditorGUILayout.FloatField("Min Height",myScript.minheight);
        myScript.maxheight = EditorGUILayout.FloatField("Max Height", myScript.maxheight);
        myScript.height = EditorGUILayout.Slider("Height", myScript.height, myScript.minheight, myScript.maxheight);
        myScript.Jumpforce = EditorGUILayout.FloatField("Jump Force", myScript.Jumpforce);
        myScript.KeepVerticalSpeed = GUILayout.Toggle(myScript.KeepVerticalSpeed, "Keep Vertical Speed ");

        myScript.lostcontrol = GUILayout.Toggle(myScript.lostcontrol, "Lost Control");

       

        if (myScript.KeepVerticalSpeed)
            myScript.KeepVerticalSpeed = true;
        if (myScript.KeepVerticalSpeed==false)
            myScript.KeepVerticalSpeed = false;


        if (myScript.lostcontrol) {
            myScript.temporaly = GUILayout.Toggle(myScript.temporaly, "Lost Control Temporaly");
            
        }
        if (myScript.temporaly) {

            myScript.time = EditorGUILayout.Slider("Time", myScript.time, 0.1f, 1f);

        }
        if (myScript.temporaly==false)
        {

            myScript.time = 0.0f ;

        }
    }
}

