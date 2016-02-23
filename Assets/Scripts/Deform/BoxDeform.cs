using UnityEngine;
using System.Collections;

public class BoxDeform : MonoBehaviour {
	
	public Mesh originalMesh;

	[Range(0, 2)]
	public float deformation = 1;

	public bool splashDeform = false;
	
	private MeshFilter _meshFilter;

	private Mesh _modifiedMesh;
	
	void Awake () {
		_meshFilter = GetComponent<MeshFilter>();
    }
	
	void Update () {
		if (deformation == 1) {
			_meshFilter.mesh = originalMesh;
			return;
		}
		
		// Clones the mesh
		_modifiedMesh = Instantiate(originalMesh);
		_modifiedMesh.MarkDynamic();

		// Calculates the mid point beetween the planes
		Bounds bounds = _modifiedMesh.bounds;
		Vector3 refPoint = bounds.center + new Vector3(-bounds.extents.x, 0, 0);

		// For each point of the mesh, applys the right offset
		Vector3[] newVertices = new Vector3[_modifiedMesh.vertexCount];
		for (int i = 0; i < newVertices.Length; ++i) {
			// Calculates de offset and separation of the point
			Vector3 distanceToCenter = _modifiedMesh.vertices[i] - refPoint;
			Vector3 reference = refPoint - bounds.center;
			float separationFactor = Mathf.Abs(reference.x);
			separationFactor += (splashDeform ? -1 : 1) * Mathf.Abs(distanceToCenter.x);
            separationFactor /= Mathf.Abs(reference.x);
			float distance = distanceToCenter.x;
			distanceToCenter.x = 0;

			// Calculates the offset and adds it to the point
			Vector3 offset = distanceToCenter * Smooth(separationFactor) * (1 - deformation);
			newVertices[i] = _modifiedMesh.vertices[i] + offset;

			// Stretches the point
			newVertices[i].x = refPoint.x + distance * deformation;
		}

		// Assigns the new points
		_modifiedMesh.vertices = newVertices;
		_modifiedMesh.RecalculateNormals();
		_modifiedMesh.uv = originalMesh.uv;

		// Assigns the new mesh
		_meshFilter.mesh = _modifiedMesh;
	}

	public void OnDrawGizmos() {
		Awake();
		Update();
	}

	private float Smooth(float x, float edge0 = 0, float edge1 = 1) {
		// Scale, bias and saturate x to 0..1 range
		x = Mathf.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
		// Evaluate polynomial
		return x * x * x * (x * (x * 6 - 15) + 10);
	}
}
