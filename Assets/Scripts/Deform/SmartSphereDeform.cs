using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Modifies a spherical mesh simulating it is fluid.
/// Deforms the mesh when colliding with other entitys.
/// This version uses more information about each ray to
/// deform the mesh more smartly.
/// </summary>
public class SmartSphereDeform : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// The mesh to deform. This mesh will be cloned so it won't be
	/// directly modified.
	/// </summary>
	public Mesh originalMesh;

	/// <summary>
	/// The collider containing the size of the mesh.
	/// </summary>
	public SphereCollider sphereCollider;

	/// <summary>
	/// The layer which the mesh will deform with.
	/// </summary>
	public LayerMask layer;

	/// <summary>
	/// Number of rays to cast. The more rays, the more realistic and
	/// reliable the deformation will be, but the performance cost is
	/// increased.
	/// </summary>
	public int numberOfRays = 8;

	/// <summary>
	/// The distance the deformation can penetrate into the mesh.
	/// </summary>
	public float penetrationDistance = 0.05f;

	/// <summary>
	/// Speed of the vertices when moving to the modified position.
	/// </summary>
	[Range(0,1)]
	public float deformationSpeed = 0.25f;

	public float chamfFactor = 1;

	#endregion

	#region Private Attributes

	/// <summary>
	/// Reference to the entity's mesh filter component.
	/// </summary>
	private MeshFilter _meshFilter;

	/// <summary>
	/// Reference to the entity's transfom component.
	/// </summary>
	private Transform _transform;

	/// <summary>
	/// The modified mesh that would be deformed.
	/// </summary>
	private Mesh _modifiedMesh;

	#endregion

	#region Variables

	/// <summary>
	/// The points where the deformation will take place.
	/// </summary>
	private Vector3[] _deformationPoints;

	/// <summary>
	/// The distance the deformation will affect nearby vertex.
	/// </summary>
	private float _deformationDistance;

	/// <summary>
	/// Total distance to cast the rays.
	/// </summary>
	private float _rayDistance;

	/// <summary>
	/// The origin point for each ray.
	/// </summary>
	private Vector3[] _rayOrigins;

	/// <summary>
	/// The direction for casting each ray.
	/// </summary>
	private Vector3[] _rayDirections;

	/// <summary>
	/// The amount of deformation each ray will apply to each
	/// vertex.
	/// </summary>
	private float[][] _deformationWeights;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called when the entity is created.
	/// It will be called even if the entity is disabled.
	/// Recovers the references and clones the mesh.
	/// </summary>
	void Awake() {
		// Recovers the desired components
		_meshFilter = GetComponent<MeshFilter>();
		_transform = transform;

		// Clones the mesh
		_modifiedMesh = Instantiate(originalMesh);
		_modifiedMesh.MarkDynamic();

		// Assigns the new mesh
		_meshFilter.mesh = _modifiedMesh;

		// Calculates the deformation point and their influence area
		PrecalculateDeformation();

		// Calculates the origin and direction of each ray
		PrecalculateRays();
	}

	/// <summary>
	/// Unity's method called at the end of each frame.
	/// Does the deformation.
	/// </summary>
	void LateUpdate() {
		// Applies deformation
		Deform();
	}

	/// <summary>
	/// Precalcutes all the information needed to calculate the deformation
	/// weights.
	/// </summary>
	private void PrecalculateDeformation() {
		_deformationPoints = new Vector3[numberOfRays];
		_deformationWeights = new float[numberOfRays][];
		Vector3 center = _transform.TransformPoint(sphereCollider.center);
		float angleBetweenRays = 2 * Mathf.PI / numberOfRays;
		float boundsAngle = angleBetweenRays * Mathf.Rad2Deg;
        float radius = sphereCollider.radius * _transform.lossyScale.x;
		// base = 2 * side * Cos(sideAngle) => sideAngle = (180º - baseAngle) / 2
		_deformationDistance = 2 * radius * Mathf.Cos((Mathf.PI - angleBetweenRays) / 2);
		Color32[] colors = new Color32[originalMesh.vertexCount];
		for (int i = 0; i < colors.Length; i++)
			colors[i] = Color.black;
		for (int i = 0; i < numberOfRays; i++) {
			Vector3 direction = new Vector3(Mathf.Cos(i * angleBetweenRays), Mathf.Sin(i * angleBetweenRays), 0);
			_deformationPoints[i] = center + radius * direction;

			_deformationWeights[i] = new float[originalMesh.vertexCount];
			float scaledPenetrationDistance = penetrationDistance * _transform.lossyScale.x;
            float maxZ = Mathf.Sqrt(scaledPenetrationDistance * (2 * radius - scaledPenetrationDistance));
            for (int j = 0; j < originalMesh.vertexCount; j++) {
				Vector3 distanceToVertex = _transform.TransformPoint(originalMesh.vertices[j]) - center;
				float zDistance = Mathf.Abs(distanceToVertex.z);
				distanceToVertex.z = 0;
                float angle = Vector3.Angle(distanceToVertex, direction);
				if (Mathf.Abs(angle) <= boundsAngle && zDistance <= maxZ) {
					float zFactor = 1 - zDistance / maxZ;
                    _deformationWeights[i][j] = (1 - angle / boundsAngle) * Mathf.Sqrt(zFactor);
				}
				else
					_deformationWeights[i][j] = 0;
				colors[j] += _deformationWeights[i][j] * (Color.red * ((i + 1) / (float)numberOfRays) + Color.blue * (1 - (i + 1) / (float)numberOfRays));
            }
		}
		originalMesh.colors32 = colors;
	}

	/// <summary>
	/// Precalculates all the information needed to cast the rays:
	/// origin, direction and distance.
	/// </summary>
	private void PrecalculateRays() {
		_rayOrigins = new Vector3[numberOfRays];
		_rayDirections = new Vector3[numberOfRays];
		Vector3 center = _transform.TransformPoint(sphereCollider.center);
		float angleBetweenRays = 2 * Mathf.PI / numberOfRays;
		float radius = (sphereCollider.radius - penetrationDistance) * _transform.lossyScale.x;
		for (int i = 0; i < numberOfRays; i++) {
			Vector3 direction = new Vector3(Mathf.Cos(i * angleBetweenRays), Mathf.Sin(i * angleBetweenRays), 0); ;
			_rayOrigins[i] = center + radius * direction;
			_rayDirections[i] = direction;
		}
		_rayDistance = penetrationDistance * transform.lossyScale.x;
	}

	/// <summary>
	/// Casts the rays and starts the deformation.
	/// </summary>
	private void Deform() {
		Vector3[] modifiedVertices = originalMesh.vertices;
		for (int i = 0; i < numberOfRays; i++) {
			// Casts the ray. If the ray didn't hit anything, skips it
			RaycastHit hitInfo;
			if (!Physics.Raycast(_rayOrigins[i], _rayDirections[i], out hitInfo, _rayDistance, layer))
				continue;
			Debug.DrawRay(_rayOrigins[i], _rayDirections[i] * _rayDistance, Color.blue);
			
			// Calculates the deformation
			Vector3 deformation = _rayDirections[i] * -(_rayDistance - hitInfo.distance);
            deformation = Vector3.Project(deformation, hitInfo.normal);

			// For each vertex, calculates the deformation to apply
			DeformVertices(i, deformation, ref modifiedVertices);
        }

		// Moves the vertices to their desired position
		Vector3[] newVertices = new Vector3[modifiedVertices.Length];
		for (int i = 0; i < newVertices.Length; i++)
			newVertices[i] = Vector3.Lerp(_modifiedMesh.vertices[i], modifiedVertices[i], deformationSpeed);

		// Reassignates the vertices and recalculates the normals of the vertices
		_modifiedMesh.vertices = newVertices;
		_modifiedMesh.RecalculateNormals();
	}

	public float chamfPenetration = 1;
	public float chamfScale = 0.15f;

	/// <summary>
	/// Calculates the deformation derived from a single ray.
	/// </summary>
	/// <param name="rayIndex">Index of the ray causing the deformation</param>
	/// <param name="deformation">Deformation performed by the ray</param>
	/// <param name="vertices">The vertices to deform</param>
	/// <returns></returns>
	private void DeformVertices(int rayIndex, Vector3 deformation, ref Vector3[] vertices) {
		for (int i = 0; i < vertices.Length; i++) {
			// Calculates the global vertex position
			Vector3 vertexGlobalPosition = _transform.TransformPoint(vertices[i]);
			Vector3 vertexDistance = vertexGlobalPosition - _deformationPoints[rayIndex];

			// Adds the deformation to the vertex
			vertexGlobalPosition += deformation * _deformationWeights[rayIndex][i];

			Vector3 distanceProjection = Vector3.Project(vertexDistance, deformation);
			if (distanceProjection.sqrMagnitude > (chamfPenetration * deformation).sqrMagnitude)
				continue;
			
			// Moves the vertex to keep the volume
			Vector3 offset = Vector3.ProjectOnPlane(vertexDistance, deformation);
			offset *= (chamfPenetration * deformation - distanceProjection).magnitude;
			offset *= chamfScale;
			vertexGlobalPosition += _transform.InverseTransformDirection(offset) / (_transform.lossyScale.x * _transform.lossyScale.x);

			// Returns the vertex to its position
			vertices[i] = _transform.InverseTransformPoint(vertexGlobalPosition);
		}
	}

	#endregion
}
