using UnityEngine;

/// <summary>
/// Modifies a spherical mesh simulating it is fluid.
/// Deforms the mesh when colliding with other entitys.
/// This version uses more information about each ray to
/// deform the mesh more smartly.
/// </summary>
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class SkinnedSphereDeform : MonoBehaviour {

	#region Public Attributes

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
	public int numberOfRays = 32;

	/// <summary>
	/// Speed of the vertices when moving to the modified position.
	/// </summary>
	public float deformationSpeed = 10;

	/// <summary>
	/// The amount of chamf deformation applied to nearby vertices.
	/// </summary>
	public float chamfScale = 1;

	#endregion

	#region Private Attributes

	/// <summary>
	/// Reference to the entity's skinned mesh renderer component.
	/// </summary>
	private SkinnedMeshRenderer _skinnedMeshRenderer;

	/// <summary>
	/// Reference to the entity's transfom component.
	/// </summary>
	private Transform _transform;

	/// <summary>
	/// The mesh without deformation.
	/// </summary>
	private Mesh _originalMesh;

	/// <summary>
	/// The modified mesh that will be deformed.
	/// </summary>
	private Mesh _modifiedMesh;

	#endregion

	#region Variables

	/// <summary>
	/// The points where the deformation will take place.
	/// </summary>
	private Vector3[] _deformationPoints;

	/// <summary>
	/// The direction for casting each ray.
	/// </summary>
	private Vector3[] _rayDirections;

	/// <summary>
	/// A reference to the original positions of the vertices.
	/// </summary>
	private Vector3[] _originalVertices;

	/// <summary>
	/// The number of rays casted on the previous frame.
	/// </summary>
	private int _lastFrameNumberOfRays;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called when the entity is created.
	/// Recovers the references and clones the mesh.
	/// </summary>
	void Start() {
		// Recovers the desired components
		_skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		_transform = transform;

		// Clones the mesh
		_originalMesh = _skinnedMeshRenderer.sharedMesh;
		_modifiedMesh = Instantiate(_originalMesh);
		_modifiedMesh.MarkDynamic();

		// Assigns the new mesh
		_skinnedMeshRenderer.sharedMesh = _modifiedMesh;

		// Precalculates some data
		PrecalculateData();
		_lastFrameNumberOfRays = numberOfRays;
	}

	/// <summary>
	/// Unity's method called at the end of each frame.
	/// Does the deformation.
	/// </summary>
	void LateUpdate() {
		// If the number of rays has been modified, updates the precalculations
		if (_lastFrameNumberOfRays != numberOfRays) {
			PrecalculateData();
			_lastFrameNumberOfRays = numberOfRays;
		}

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
		_rayDirections = new Vector3[numberOfRays];
		_originalVertices = _originalMesh.vertices;

		// Calculates the temporary information needed
		Vector3 center = sphereCollider.center;
		float angleBetweenRays = 2 * Mathf.PI / numberOfRays;

		// For each ray, precalculates the information
		for (int i = 0; i < numberOfRays; i++) {
			// Calculates the ray direction
			_rayDirections[i] = new Vector3(Mathf.Cos(i * angleBetweenRays), Mathf.Sin(i * angleBetweenRays), 0);

			// Calculates the deformation point of the ray
			_deformationPoints[i] = center + sphereCollider.radius * _rayDirections[i];
		}
	}
    
	/// <summary>
	/// Casts the rays and starts the deformation.
	/// </summary>
	private void Deform() {
		// Initializes the arrays
		Vector3[] modifiedVertices = (Vector3[])_originalVertices.Clone();

		// Calculates the needed information
		Vector3 center = _transform.TransformPoint(sphereCollider.center);
		float rayDistance = sphereCollider.radius * _transform.lossyScale.x;

		// Casts the rays
		Vector3[] deformations = new Vector3[numberOfRays];
		for (int i = 0; i < numberOfRays; i++) {
			// If the ray didn't hit anything, skips it
			RaycastHit hitInfo;
			if (!Physics.Raycast(center, _rayDirections[i], out hitInfo, rayDistance, layer)) {
				deformations[i] = Vector3.zero;
				continue;
			}

			// Calculates the deformation
			deformations[i] = _rayDirections[i] * (hitInfo.distance - rayDistance);
			deformations[i] = Vector3.Project(deformations[i], hitInfo.normal);

			// For each vertex, calculates the chamf deformation to apply
			DeformChamfVertices(i, deformations[i], _originalVertices, ref modifiedVertices);
		}

		// For each vertex, calculates the planar deformation to apply
		for (int i = 0; i < numberOfRays; i++)
			if (deformations[i] != Vector3.zero)
				DeformPlanarVertices(i, deformations[i], _originalVertices, ref modifiedVertices);

		// Moves the vertices to their desired position
		Vector3[] newVertices = new Vector3[modifiedVertices.Length];
		Vector3[] lastFrameVertices = _modifiedMesh.vertices;
		for (int i = 0; i < newVertices.Length; i++)
			newVertices[i] = Vector3.Lerp(lastFrameVertices[i], modifiedVertices[i], deformationSpeed * Time.deltaTime);
		
		// Reassignates the vertices
		_modifiedMesh.vertices = newVertices;
	}

	/// <summary>
	/// Calculates the chamf deformation derived from a single ray, perpendicular
	/// to the deformation distance.
	/// </summary>
	/// <param name="rayIndex">Index of the ray causing the deformation</param>
	/// <param name="deformation">Deformation performed by the ray</param>
	/// <param name="vertexToModify">A reference to the vertices to modify</param>
	private void DeformChamfVertices(int rayIndex, Vector3 deformation, Vector3[] originalVertices, ref Vector3[] vertexToModify) {
		// Precalculates some data
		Vector3 center = _transform.TransformPoint(sphereCollider.center);
		float deformationFactor = chamfScale;
		deformationFactor /= sphereCollider.radius * _transform.lossyScale.x * Mathf.Sqrt(numberOfRays);
		Vector3 chamfDirection = Vector3.Cross(deformation, Vector3.forward);
		float deformationMagnitudeFactor = 1.0f / deformation.magnitude;

		Vector3 distanceToVertex;
		Vector3 chamfDeformation;
		float length = vertexToModify.Length;

		// For each vertex, calculates it's deformation
		for (int i = 0; i < length; ++i) {
			// Calculates the distance from the center to the vertex in global coordinates
			distanceToVertex = _transform.TransformPoint(originalVertices[i]);
			distanceToVertex -= center;

			// Calculates the deformation to apply
			chamfDeformation = chamfDirection;
			chamfDeformation *= Vector3.Dot(chamfDirection, distanceToVertex);
			chamfDeformation *= deformationMagnitudeFactor;

			// Applys the chamf to the vertex
			vertexToModify[i] += _transform.InverseTransformVector(chamfDeformation * deformationFactor);
		}
	}

	/// <summary>
	/// Calculates the deformation on the deformation direction derived
	/// from a single ray, compensating the chamf.
	/// </summary>
	/// <param name="rayIndex">Index of the ray causing the deformation</param>
	/// <param name="deformation">Deformation performed by the ray</param>
	/// <param name="vertexToModify">A reference to the vertices to modify</param>
	private void DeformPlanarVertices(int rayIndex, Vector3 deformation, Vector3[] originalVertices, ref Vector3[] vertexToModify) {
		// Precalculates some data
		Vector3 center = _transform.TransformPoint(sphereCollider.center);
		float radiusFactor = 1.0f / (sphereCollider.radius * _transform.lossyScale.x);
		float angleBetweenRays = 2 * Mathf.PI / numberOfRays;
		float angleBetweenRaysFactor = 1.0f / angleBetweenRays;
		float rayAngle = Mathf.Atan2(_rayDirections[rayIndex].y, _rayDirections[rayIndex].x);

		Vector3 distanceToVertex;
		float angle;
		float weight;
        float length = vertexToModify.Length;

		// For each vertex, calculates it's deformation
		for (int i = 0; i < length; ++i) {
			// Calculates the distance from the center to the vertex in global coordinates
			distanceToVertex = _transform.TransformPoint(originalVertices[i]);
			distanceToVertex -= center;

			// Checks if the vertex is near the ray using the angle between rays
			angle = Mathf.Abs(rayAngle - Mathf.Atan2(distanceToVertex.y, distanceToVertex.x));
			if (angle <= angleBetweenRays) {
				// Calculates the weight of the vertex
				weight = 1;
				weight -= Mathf.Abs(distanceToVertex.z) * radiusFactor;
				weight *= 1 - angle * angleBetweenRaysFactor;

				// Adds the deformation to the vertex
				vertexToModify[i] += _transform.InverseTransformVector(deformation * weight);
			}
		}
	}

	#endregion
}
