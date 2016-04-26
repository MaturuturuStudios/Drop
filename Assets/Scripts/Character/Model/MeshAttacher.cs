using UnityEngine;

public class MeshAttacher : MonoBehaviour {

	public Transform meshObject;

	private SkinnedMeshRenderer skinnedMeshRenderer;

	private int vertexIndex;

	private Vector3 offset;

	// Use this for initialization
	void Start () {
		skinnedMeshRenderer = meshObject.GetComponent<SkinnedMeshRenderer>();
		Mesh bakedMesh = new Mesh();
		skinnedMeshRenderer.BakeMesh(bakedMesh);
		Vector3[] vertices = bakedMesh.vertices;
		float nearestDistance = float.MaxValue;
		Vector3 originalPosition = transform.position;
		for (int i = 0; i < vertices.Length; i++) {
			Vector3 vertexPosition = meshObject.TransformPoint(vertices[i]);
			float distance = Vector3.Distance(originalPosition, vertexPosition);
			if (distance < nearestDistance) {
				nearestDistance = distance;
				vertexIndex = i;
			}
		}
		offset = meshObject.TransformPoint(vertices[vertexIndex]) - originalPosition;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Mesh bakedMesh = new Mesh();
		skinnedMeshRenderer.BakeMesh(bakedMesh);
		Vector3 vertexPosition = bakedMesh.vertices[vertexIndex];
		Debug.DrawLine(meshObject.position, vertexPosition);
		transform.position = vertexPosition + transform.TransformVector(offset);
	}
}
