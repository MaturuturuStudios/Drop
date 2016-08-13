using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Scene))]
public class ScenePropertyDrawer : PropertyDrawer {
	//size of help box
	private float heightHelpBox = EditorGUIUtility.singleLineHeight * 2.5f;
	//size of field
	private float heightField = EditorGUIUtility.singleLineHeight;

	//the suffix of a scene
	private string sceneSuffix = " (SceneAsset)";
	//value on the field
	private Object original = null;
	//was an error asigning?
	private bool error = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
		// Don't make child fields be indented
		//int indent = EditorGUI.indentLevel;
		//EditorGUI.indentLevel = 0;

        //get data
        SerializedProperty sceneName;
        SerializedProperty sceneObject;

        sceneName = property.FindPropertyRelative("name");
        sceneObject = property.FindPropertyRelative("sceneObject");


		Object newScene = sceneObject.objectReferenceValue;
		if (newScene != original) {
			error = false;
			if (IsValid (newScene)) {
				sceneObject.objectReferenceValue = newScene;
				original = newScene;

				//get the name of scene/object
				string fullName = ObjectNames.GetDragAndDropTitle (original);
				sceneName.stringValue = fullName.Substring (0, fullName.Length - sceneSuffix.Length);

			} else {
				if (newScene != null) {
					error = true;
				}

				//set the name as empty, no scene
				sceneName.stringValue = "";
				sceneObject.objectReferenceValue = null;
				original = null;
			}
		}



		//Draw everything
		Rect positionLabel=position;
		Rect positionHelp = position;
		if (error) {
			positionLabel.height = heightField;

			positionHelp.height = heightHelpBox;
			//positionHelp.width += positionHelp.x * 2;
			//positionHelp.x = 0;
			positionHelp.y += heightField;
		}

		DrawField (positionLabel, sceneObject, label);
		DrawHelpBox (positionHelp);
        
        // Set indent back to what it was
        //EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

	/// <summary>
	/// Gets the height of the property.
	/// </summary>
	/// <returns>The property height.</returns>
	/// <param name="prop">Property.</param>
	/// <param name="label">Label.</param>
	public override float GetPropertyHeight (SerializedProperty prop, GUIContent label) {
		if (error) 
			return heightHelpBox + heightField;
		else
			return heightField;
	}

	/// <summary>
	/// Draws the field.
	/// </summary>
	/// <param name="position">Position.</param>
	/// <param name="sceneObject">Scene object.</param>
	/// <param name="label">Label.</param>
	private void DrawField(Rect position, SerializedProperty sceneObject, GUIContent label){
		// Draw label
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		//draw field
		EditorGUI.ObjectField(position, sceneObject, GUIContent.none);
	}


	/// <summary>
	/// Draws the help box.
	/// </summary>
	/// <param name="position">Position.</param>
	private void DrawHelpBox (Rect position) {
		// No need for a help box if no error.
		if (!error)
			return;
		
		EditorGUI.HelpBox (position, "That is not a scene!", MessageType.Error);
	}

	/// <summary>
	/// Determines whether this value field is valid.
	/// </summary>
	/// <returns><c>true</c> if this instance is valid; otherwise, <c>false</c>.</returns>
	private bool IsValid(Object value){
		if (value == null)
			return true;

		//get the name of scene/object
		string fullName = ObjectNames.GetDragAndDropTitle(value);
		//check if is a scene
		return fullName.EndsWith(sceneSuffix);

	}
}