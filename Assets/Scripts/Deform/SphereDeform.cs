using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Modifies a spherical mesh simulating it is fluid.
/// Deforms the mesh when colliding with other entitys.
/// </summary>
public class SphereDeform : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// The mesh to deform. This mesh will be cloned so it won't be
	/// directly modified.
	/// </summary>
	public Mesh originalMesh;

	/// <summary>
	/// The collider containing the minimum size the mesh can have.
	/// </summary>
	public SphereCollider sphereCollider;

	/// <summary>
	/// The layer which the mesh will deform with.
	/// </summary>
	public LayerMask layer;

	/// <summary>
	/// Number of rays to cast in order to detect the other colliders.
	/// </summary>
	public int numberOfRays = 8;

	/// <summary>
	/// The distance the rays will check for collisions.
	/// </summary>
	public float distance = 0.05f;

	/// <summary>
	/// Inner distance to cast the rays. Used to improve reliability.
	/// </summary>
	public float skinWidth = 0.02f;

	/// <summary>
	/// Speed of the vertices when moving to the modified position.
	/// </summary>
	[Range(0,1)]
	public float deformationSpeed = 0.25f;

	/// <summary>
	/// If the deformation should affect nearby vertices.
	/// </summary>
    public bool chamf = true;

	/// <summary>
	/// The amount of deformation to apply to the vertices.
	/// </summary>
	public float chamfScale = 0.25f;

	/// <summary>
	/// Amount of penetration to check if a nearby vertex should be
	/// deformed.
	/// </summary>
	public float chamfPenetration = 5;

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
	/// Constains wich rays have hit this frame and which have not.
	/// </summary>
	private bool[] _rayHits;

	/// <summary>
	/// Contains the information about each ray cast hit.
	/// </summary>
	private RaycastHit[] _rayHitsInfo;

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
	}

	/// <summary>
	/// Unity's method called at the end of each frame.
	/// Does the deformation.
	/// </summary>
	void LateUpdate() {
		// Calculates the origin and direction of each ray
		PrecalculateRays();

		// Casts the rays
		CastRays();

		// Applies deformation
		Deform();
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

	/// <summary>
	/// Casts the rays and stores the results.
	/// </summary>
	private void CastRays() {
		_rayHits = new bool[numberOfRays];
		_rayHitsInfo = new RaycastHit[numberOfRays];
		for (int i = 0; i < numberOfRays; i++) {
			_rayHits[i] = Physics.Raycast(_rayOrigins[i], _rayDirections[i], out _rayHitsInfo[i], _rayDistance, layer);
			Debug.DrawRay(_rayOrigins[i], _rayDirections[i] * _rayDistance, Color.blue);
		}
	}

	/// <summary>
	/// Does the actual deformation.
	/// </summary>
	private void Deform() {
		Vector3[] modifiedVertices = originalMesh.vertices;
		Vector3[][] chamfDeform = new Vector3[numberOfRays][];
		Dictionary<Collider, int> colliderHitCount = new Dictionary<Collider, int>();
		for (int i = 0; i < numberOfRays; i++) {
			// If the ray didn't hit anything, skips it
			if (!_rayHits[i])
				continue;

			// Increases the count of collisions with the collider
			Collider hitCollider = _rayHitsInfo[i].collider;
			if (colliderHitCount.ContainsKey(hitCollider))
				colliderHitCount[hitCollider] += 1;
			else
				colliderHitCount.Add(hitCollider, 1);

			// Gets the distance to deform
			Vector3 deformation = _rayDirections[i] * -(_rayDistance - _rayHitsInfo[i].distance);
            deformation = Vector3.Project(deformation, _rayHitsInfo[i].normal);

			// Gets the origin of the deform
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
						if (chamfDeform[i][j] != null) {
							modifiedVertices[j] += chamfDeform[i][j] / colliderHitCount[_rayHitsInfo[i].collider];
						}
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

	/// <summary>
	/// Calculates the deformation derived from a single ray.
	/// </summary>
	/// <param name="origin">Point in the mesh where the ray passes through</param>
	/// <param name="deformation">Deformation performed by the ray</param>
	/// <param name="vertices">The vertices to deform</param>
	/// <param name="chamfFator">Factor of deformation caused by the ray</param>
	/// <returns></returns>
	private Vector3[] DeformVertices(Vector3 origin, Vector3 deformation, ref Vector3[] vertices, float chamfFator) {
		Vector3[] chamfDeform = new Vector3[vertices.Length];
		for (int i = 0; i < vertices.Length; i++) {
			// Calculates the global vertex position
			Vector3 vertexGlobalPosition = _transform.TransformPoint(vertices[i]);

			// Calculates the distance from the origin to the vertex
			Vector3 vertexDistance = vertexGlobalPosition - origin;

			// Projects the distance into the deformation vector
			Vector3 distanceProjection = Vector3.Project(vertexDistance, deformation);

			// If the distance is greater than the chamf penetration times the deformation, skips the vertex
			float penetration = chamfPenetration / transform.lossyScale.x;
            if (distanceProjection.sqrMagnitude > (penetration * deformation).sqrMagnitude)
				continue;

			// If the distance is lower than the deformation, moves the vertex
			if (distanceProjection.sqrMagnitude < deformation.sqrMagnitude)
				vertices[i] = _transform.InverseTransformPoint(vertexGlobalPosition + deformation - distanceProjection);

			// If the champ flag is set, moves the near vertex to keep the volume
			if (chamf) {
				// Moves the vertex to keep the volume
				Vector3 offset = Vector3.ProjectOnPlane(vertexDistance, deformation);
				offset *= (penetration * deformation - distanceProjection).magnitude;
				offset *= chamfScale * chamfFator;
                chamfDeform[i] = _transform.InverseTransformDirection(offset) / (_transform.lossyScale.x * _transform.lossyScale.x);
			}
		}
		return chamfDeform;
	}

	#endregion
}
