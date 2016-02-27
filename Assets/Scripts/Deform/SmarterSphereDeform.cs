using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Modifies a spherical mesh simulating it is fluid.
/// Deforms the mesh when colliding with other entitys.
/// This version uses more information about each ray to
/// deform the mesh more smartly.
/// </summary>
public class SmarterSphereDeform : MonoBehaviour {

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
	/// Speed of the vertices when moving to the modified position.
	/// </summary>
	[Range(0,1)]
	public float deformationSpeed = 0.25f;

	/// <summary>
	/// The amount of chamf deformation applied to nearby vertices.
	/// </summary>
	public float chamfScale = 1;

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
	/// The amount of deformation each ray will apply to each
	/// vertex.
	/// </summary>
	private float[][] _deformationWeights;

	/// <summary>
	/// The direction for casting each ray.
	/// </summary>
	private Vector3[] _rayDirections;

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

		// Precalculates all the necessary information about
		PrecalculateData();
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
	/// weights and cast the rays.
	/// </summary>
	private void PrecalculateData() {
		// Initializes the arrays
		_deformationPoints = new Vector3[numberOfRays];
		_deformationWeights = new float[numberOfRays][];
		_rayDirections = new Vector3[numberOfRays];
		
		// Calculates the temporary information needed
		Vector3 center = sphereCollider.center;
		float angleBetweenRays = 2 * Mathf.PI / numberOfRays;
		
		// For each ray, precalculates the information
		for (int i = 0; i < numberOfRays; i++) {
			// Calculates the ray direction
			_rayDirections[i] = new Vector3(Mathf.Cos(i * angleBetweenRays), Mathf.Sin(i * angleBetweenRays), 0);

			// Calculates the deformation point of the ray
			_deformationPoints[i] = center + sphereCollider.radius * _rayDirections[i];

			// Calculates the deformation weights of the ray
			_deformationWeights[i] = new float[originalMesh.vertexCount];
            for (int j = 0; j < originalMesh.vertexCount; j++) {
				// Calculates the distance from the center to the vertex in global coordinates
				Vector3 distanceToVertex = originalMesh.vertices[j] - center;

				// Isolates the Z coordinate, as the rays are not casted in that direction
				float zDistance = Mathf.Abs(distanceToVertex.z);
				distanceToVertex.z = 0;

				// Checks if the vertex is near the ray using the angle between rays
                float angle = Vector3.Angle(distanceToVertex, _rayDirections[i]) * Mathf.Deg2Rad;
				if (Mathf.Abs(angle) <= angleBetweenRays) {
					// Calculates the weight of the vertex
					float zFactor = 1 - zDistance / sphereCollider.radius;
                    _deformationWeights[i][j] = (1 - angle / angleBetweenRays) * Mathf.Sqrt(zFactor);
				}
				else
					_deformationWeights[i][j] = 0;
            }
		}
	}

	/// <summary>
	/// Casts the rays and starts the deformation.
	/// </summary>
	private void Deform() {
		// Initializes the arrays
		Vector3[] modifiedVertices = originalMesh.vertices;

		// Calculates the needed information
		Vector3 center = _transform.TransformPoint(sphereCollider.center);
		float rayDistance = sphereCollider.radius * _transform.lossyScale.x;

		// Casts the rays
		for (int i = 0; i < numberOfRays; i++) {
			// If the ray didn't hit anything, skips it
			RaycastHit hitInfo;
			Vector3 transformedDirection = _transform.TransformDirection(_rayDirections[i]);
            if (!Physics.Raycast(center, transformedDirection, out hitInfo, rayDistance, layer))
				continue;
			Debug.DrawRay(center, transformedDirection * rayDistance, Color.blue);

			// Calculates the deformation
			Vector3 deformation = transformedDirection * (hitInfo.distance - rayDistance);
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

	/// <summary>
	/// Calculates the deformation derived from a single ray.
	/// </summary>
	/// <param name="rayIndex">Index of the ray causing the deformation</param>
	/// <param name="deformation">Deformation performed by the ray</param>
	/// <param name="vertexToModify">A reference to the vertices to modify</param>
	private void DeformVertices(int rayIndex, Vector3 deformation, ref Vector3[] vertexToModify) {
		// Initializes the arrays
		Vector3[] originalVertices = originalMesh.vertices;

		// Calculates the volume of the sphere
		float spherevolume = 4.0f * Mathf.PI * sphereCollider.radius * sphereCollider.radius * sphereCollider.radius / 3.0f;

		// For each vertex, calculates it's deformation
		for (int i = 0; i < vertexToModify.Length; i++) {
			// Calculates the global vertex position
			Vector3 vertexGlobalPosition = _transform.TransformPoint(originalVertices[i]);
			Vector3 vertexDistance = vertexGlobalPosition - _transform.TransformPoint(_deformationPoints[rayIndex]);

			// Adds the deformation to the vertex
			Vector3 appliedDeformation = deformation * _deformationWeights[rayIndex][i];


			// Applys the chamf to the vertex
			Vector3 distanceProjection = Vector3.ProjectOnPlane(vertexDistance, deformation);
			Vector3 offset = distanceProjection.normalized;
			offset *= distanceProjection.magnitude / (sphereCollider.radius * _transform.lossyScale.x);
            offset *= deformation.magnitude;
			offset *= spherevolume * spherevolume;
			offset *= chamfScale;
			appliedDeformation += offset;

			// Returns the vertex to its position
			vertexToModify[i] += _transform.InverseTransformVector(appliedDeformation);
		}
	}

	#endregion
}
