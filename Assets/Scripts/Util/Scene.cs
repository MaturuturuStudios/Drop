using UnityEngine;

[System.Serializable]
public class Scene {
	
    [SerializeField]
	//disable the warning of field assigned but not used
	#pragma warning disable 0414
    /// <summary>
    /// The scene object. Used only on inspector
    /// </summary>
	private Object sceneObject = null;
	#pragma warning restore 0414

	/// <summary>
	/// The name of the scene.
	/// </summary>
    public string name = "";
}