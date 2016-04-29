using UnityEngine;

/// <summary>
/// Makes an entity follow the closest point in a mesh.
/// </summary>
public class MeshAttacher : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// Object to attach to the vertex.
	/// </summary>
	public Transform meshObject;

	/// <summary>
	/// If enabled, the initial offset to the vertex will
	/// be kept.
	/// </summary>
	public bool keepOffset = true;

	#endregion

	#region Private Attributes

	/// <summary>
	///  A reference to the entity's Transform component.
	/// </summary>
	private Transform _transform;

	/// <summary>
	/// A reference to the attached entity SkinnedMeshRenderer's
	/// component.
	/// </summary>
	private SkinnedMeshRenderer _skinnedMeshRenderer;

	/// <summary>
	/// Index of the closest vertex to the point.
	/// </summary>
	private int vertexIndex;

	/// <summary>
	/// Distance to keep to the target vertex.
	/// </summary>
	private Vector3 offset;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called at the beginning of the first
	/// frame this object's active.
	/// </summary>
	void Start () {
		// Retrieves the desired components
		_transform = transform;
		_skinnedMeshRenderer = meshObject.GetComponent<SkinnedMeshRenderer>();
		if (_skinnedMeshRenderer == null) {
			Debug.LogError("Error no SkinnedMeshRenderer found on the entity!");
			enabled = false;
			return;
		}

		// Gets the vertices positions
		Mesh bakedMesh = new Mesh();
		_skinnedMeshRenderer.BakeMesh(bakedMesh);
		Vector3[] vertices = bakedMesh.vertices;

		// Finds the nearest vertex
		float nearestDistance = float.MaxValue;
		Vector3 originalPosition = _transform.position;
		for (int i = 0; i < vertices.Length; i++) {
			float distance = Vector3.Distance(originalPosition, GetVertexPosition(bakedMesh.vertices[i]));
			if (distance < nearestDistance) {
				nearestDistance = distance;
				vertexIndex = i;
			}
		}

		// Saves the offset
		offset = GetVertexPosition(bakedMesh.vertices[vertexIndex]) - originalPosition;
	}
	
	/// <summary>
	/// Unity's method called at the end of each frame.
	/// </summary>
	void LateUpdate () {
		// Adjusts the entity's position to the vertex's.
		Mesh bakedMesh = new Mesh();
		_skinnedMeshRenderer.BakeMesh(bakedMesh);
		Vector3 position = GetVertexPosition(bakedMesh.vertices[vertexIndex]);
		if (keepOffset)
			position += meshObject.TransformVector(offset);
		_transform.position = position;
	}

	/// <summary>
	/// Returns the position of a vertex in global space.
	/// Won't uses the scale, as this seems to be broken.
	/// </summary>
	/// <param name="vertex">The vertex which position will be returned</param>
	/// <returns></returns>
	private Vector3 GetVertexPosition(Vector3 vertex) {
		Vector3 vertexPosition = meshObject.position;
		vertexPosition += meshObject.TransformDirection(vertex);
		return vertexPosition;
	}

	#endregion
}
