using UnityEngine;
using System.Collections;

public class SphereDeform : MonoBehaviour {
	
	public Mesh originalMesh;
	public SphereCollider sphereCollider;
	public LayerMask layer;

	public int numberOfRays = 8;
	public float distance = 0.05f;
	public float skinWidth = 0.02f;
	[Range(0,1)]
	public float deformationSpeed = 0.25f;
    public bool chamf = true;
	public float chamfScale = 0.25f;
	public float chamfPenetration = 5;

	private MeshFilter _meshFilter;
	private Transform _transform;

	private Mesh _modifiedMesh;
	private float _rayDistance;
	private Vector3[] _rayOrigins;
	private Vector3[] _rayDirections;
	private bool[] _rayHits;
	private RaycastHit[] _rayHitsInfo;

	void Awake() {
		_meshFilter = GetComponent<MeshFilter>();
		_transform = transform;

		// Clones the mesh
		_modifiedMesh = Instantiate(originalMesh);
		_modifiedMesh.MarkDynamic();

		// Assigns the new mesh
		_meshFilter.mesh = _modifiedMesh;
	}

	void Update() {
		// Calculates the origin and direction of each ray
		PrecalculateRays();

		// Casts the rays
		CastRays();

		// Applies deformation
		Deform();
	}

	private void PrecalculateRays() {
		_rayOrigins = new Vector3[numberOfRays];
		_rayDirections = new Vector3[numberOfRays];
		Vector3 center = _transform.TransformPoint(sphereCollider.center);
		float angleBetweenRays = 2 * Mathf.PI / numberOfRays;
		float skinRadius = (sphereCollider.radius - skinWidth) * _transform.lossyScale.x;
		for (int i = 0; i < numberOfRays; i++) {
			Vector3 direction = new Vector3(Mathf.Cos(i * angleBetweenRays), Mathf.Sin(i * angleBetweenRays), 0); ;
			float x = center.x + skinRadius * direction.x;
			float y = center.y + skinRadius * direction.y;
			_rayOrigins[i] = new Vector3(x, y, 0);
			_rayDirections[i] = direction;
		}
		_rayDistance = (distance + skinWidth) * transform.lossyScale.x;
	}

	private void CastRays() {
		_rayHits = new bool[numberOfRays];
		_rayHitsInfo = new RaycastHit[numberOfRays];
		for (int i = 0; i < numberOfRays; i++) {
			_rayHits[i] = Physics.Raycast(_rayOrigins[i], _rayDirections[i], out _rayHitsInfo[i], _rayDistance, layer);
			Debug.DrawRay(_rayOrigins[i], _rayDirections[i] * _rayDistance, Color.blue);
		}
	}

	private void Deform() {
		Vector3[] modifiedVertices = originalMesh.vertices;
		Vector3[][] chamfDeform = new Vector3[numberOfRays][];
		for (int i = 0; i < numberOfRays; i++) {
			// If the ray didn't hit anything, skips it
			if (!_rayHits[i])
				continue;

			// Gets the distance to deform
			float deformDistance = _rayDistance - _rayHitsInfo[i].distance;
            Vector3 deformation = _rayHitsInfo[i].normal * deformDistance;
			Vector3 origin = _rayOrigins[i] + _rayDirections[i] * _rayDistance;

			// Calculates the chamf factor
			float chamfFactor = Mathf.Abs(Mathf.Cos(Vector3.Angle(_rayDirections[i], deformation)));
			chamfFactor *= chamfFactor;

			// For each vertex, calculates the deformation to apply
			chamfDeform[i] = DeformVertices(origin, deformation, ref modifiedVertices, chamfFactor);
        }

		// Adds all the chamf together
		if (chamf) {
			for (int i = 0; i < chamfDeform.Length; i++)
				if (chamfDeform[i] != null) {
					for (int j = 0; j < chamfDeform[i].Length; j++)
						modifiedVertices[j] += chamfDeform[i][j];
				}
		}

		// Moves the vertices to their desired position
		Vector3[] newVertices = new Vector3[modifiedVertices.Length];
		for (int i = 0; i < newVertices.Length; i++)
			newVertices[i] = Vector3.Lerp(_modifiedMesh.vertices[i], modifiedVertices[i], deformationSpeed);

		// Reassignates the vertices and recalculates the normals of the vertices
		_modifiedMesh.vertices = newVertices;
		_modifiedMesh.RecalculateNormals();
	}

	private Vector3[] DeformVertices(Vector3 origin, Vector3 deformation, ref Vector3[] vertices, float chamfFator) {
		Vector3[] chamfDeform = new Vector3[vertices.Length];
		for (int i = 0; i < vertices.Length; i++) {
			// Calculates the distance from the origin to the vertex
			Vector3 vertexPosition = _transform.TransformPoint(vertices[i]);
			Vector3 vertexDistance = vertexPosition - origin;

			// Projects the distance into the deformation vector
			Vector3 distanceProjection = Vector3.Project(vertexDistance, deformation);

			// If the distance is greater than the chamf penetration times the deformation, skips the vertex
			if (distanceProjection.sqrMagnitude > (chamfPenetration * deformation).sqrMagnitude)
				continue;

			// If the distance is lower than the deformation, moves the vertex
			if (distanceProjection.sqrMagnitude < deformation.sqrMagnitude)
				vertices[i] += (deformation - distanceProjection) / _transform.lossyScale.x;

			// If the champ flag is set, moves the near vertex to keep the volume
			if (chamf) {
				// Moves the vertex to keep the volume
				Vector3 offset = Vector3.ProjectOnPlane(vertexDistance, deformation);
				chamfDeform[i] = offset * chamfScale * chamfFator * (chamfPenetration * deformation - distanceProjection).magnitude / _transform.lossyScale.x;
			}
		}
		return chamfDeform;
	}
}
