using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ScoreLevels))]
public class ScoreLevelsDrawer : PropertyDrawer {
	/// <summary>
	/// Size of a single line
	/// </summary>
	private float heightField = EditorGUIUtility.singleLineHeight;

	/// <summary>
	/// How many levels per row in the available and unlocked list of levels inside a world
	/// </summary>
	private float fieldsPerLine = 10;

	/// <summary>
	/// If unfold section of number of levels in a world
	/// </summary>
	private bool unfoldedNumberWorld=true;
	/// <summary>
	/// If unfold section of scenes
	/// </summary>
	private bool unfoldedScenes = false;
	/// <summary>
	/// Total lines of size of the inspector
	/// </summary>
	private float totalHeight;
	/// <summary>
	/// Single pixels remains of height
	/// </summary>
	private float restHeight;

	/// <summary>
	/// Draws the information on the editor.
	/// </summary>
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);

		//reset properties
		position.height = heightField;
		totalHeight = 0;
		restHeight = 0;

		EditorGUI.LabelField (position, "Score Levels");
		position.y += heightField;
		totalHeight++;

		//get the array of number of levels in a world
		position = DrawNumberLevels(position, property);
		position.y += heightField;

		position = DrawAvailableLevels(position, property);
		position.y += heightField;

		EditorGUI.EndProperty();
	}

	/// <summary>
	/// Draw an array of numbers, the number of levels for a world
	/// control that every world has at least one level
	/// </summary>
	/// <param name="position">the actual position in inspector</param>
	/// <param name="property">the serialized property of class</param>
	/// <returns>the final position</returns>
	private Rect DrawNumberLevels(Rect position, SerializedProperty property) {
		SerializedProperty numberLevelsOnWorld = property.FindPropertyRelative("numberLevelsOnWorld");
		int numberWorld = numberLevelsOnWorld.arraySize;

		//ask number of worlds near the tittle
		Rect bufferPosition = position;
		float displacementWidth = bufferPosition.width;
		bufferPosition.width = EditorGUIUtility.fieldWidth;
		displacementWidth -= EditorGUIUtility.fieldWidth;
		bufferPosition.height = heightField;
		bufferPosition.x += displacementWidth;

		//number of world
		numberWorld = EditorGUI.IntField(bufferPosition, numberLevelsOnWorld.arraySize);
		numberLevelsOnWorld.arraySize = numberWorld;

		unfoldedNumberWorld = EditorGUI.Foldout(position, unfoldedNumberWorld, "Number of levels", false);
		totalHeight++;
		restHeight++;
		position.y ++;
		if (unfoldedNumberWorld) {
			EditorGUI.indentLevel++;
			for (int i = 0; i < numberWorld; i++) {
				totalHeight++;
				position.y += heightField;
				SerializedProperty aWorld = numberLevelsOnWorld.GetArrayElementAtIndex(i);
				if (aWorld.intValue <= 0) aWorld.intValue = 1;
				EditorGUI.DelayedIntField(position, aWorld, new GUIContent("World "+(i+1)));
				position.y++;
				restHeight++;
			}
			EditorGUI.indentLevel--;
		}

		return position;
	}

	/// <summary>
	/// Draw an array of available levels of the game
	/// </summary>
	/// <param name="position">the actual position in inspector</param>
	/// <param name="property">the serialized property of class</param>
	/// <returns>the final position</returns>
	private Rect DrawAvailableLevels(Rect position, SerializedProperty property) {
		SerializedProperty numberLevelsOnWorld = property.FindPropertyRelative("numberLevelsOnWorld");
		SerializedProperty availableLevels = property.FindPropertyRelative("maxScoreLevels");
		int numberWorld = numberLevelsOnWorld.arraySize;
		availableLevels.arraySize = numberWorld;

		unfoldedScenes = EditorGUI.Foldout(position, unfoldedScenes, "Max. score levels", true);
		totalHeight++;

		//if unfolded...
		if (unfoldedScenes) {
			//get the margin of indent
			float margin = position.x;
			EditorGUI.indentLevel++;
			margin = position.x - margin;

			//get the size for every toggle
			float width = position.width - margin * 2;
			float widthPerToggle = width / fieldsPerLine;

			//for every world...
			for (int i = 0; i < numberWorld; i++) {
				//write tittle world
				position.y += heightField;
				EditorGUI.LabelField(position, "World " + (i + 1));
				position.y++;
				restHeight++;
				totalHeight++;

				//get the levels of the world
				SerializedProperty world = availableLevels.GetArrayElementAtIndex(i)
					.FindPropertyRelative("anArray");
				//get the number of levels
				int numberLevels = numberLevelsOnWorld.GetArrayElementAtIndex(i).intValue;
				//resize it
				world.arraySize = numberLevels;

				//for every toggle/level...
				Rect bufferPosition = position;
				for (int j = 0; j < numberLevels; j++) {
					//check if need a new line (at first and every X levels)
					if (j % fieldsPerLine == 0) {
						position.y += heightField;
						bufferPosition = position;
						bufferPosition.width = widthPerToggle;
						totalHeight++;
					}
					//draw it
					EditorGUI.PropertyField(bufferPosition, world.GetArrayElementAtIndex(j), new GUIContent(""));
					bufferPosition.x += widthPerToggle;
				}
			}
			EditorGUI.indentLevel--;
		}

		return position;
	}

	/// <summary>
	/// Return the total height used in inspector
	/// </summary>
	/// <param name="property"></param>
	/// <param name="label"></param>
	/// <returns></returns>
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return totalHeight * heightField + restHeight;
	}

}