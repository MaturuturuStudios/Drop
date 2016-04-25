﻿using UnityEngine;
using System.Linq;

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

	/// <summary>
	/// If enabled, the planar deformation will compensate the chamf
	/// deformation. Note that this will prevent the entity from keeping
	/// its volume.
	/// </summary>
	public bool compensateChamf = true;

	/// <summary>
	/// Extra points that will be deformed with the mesh.
	/// </summary>
	public Transform[] extraDeformationPoints;

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

	/// <summary>
	/// Original positions of the extra deformation points.
	/// </summary>
	private Vector3[] _originalExtraPointsPositions;

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
	/// The amount of deformation each ray will apply to each
	/// extra point.
	/// </summary>
	private float[][] _extraPointsWeights;

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

		// Saves the original extra point's positions
		_originalExtraPointsPositions = extraDeformationPoints.Select(e => e.localPosition).ToArray();

		// Precalculates some data
		PrecalculateData();
		_lastFrameNumberOfRays = numberOfRays;
	}

	void OnEnable() {
		Start();
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
		_deformationWeights = new float[numberOfRays][];
		_extraPointsWeights = new float[numberOfRays][];
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

			// Calculates the deformation weights of the ray for the vertices
			_deformationWeights[i] = new float[_originalVertices.Length];
			for (int j = 0; j < _originalVertices.Length; j++) {
				// Calculates the distance from the center to the vertex in local coordinates
				Vector3 distanceToVertex = _originalVertices[j] - center;

				// Isolates the Z coordinate, as the rays are not casted in that direction
				float zDistance = Mathf.Abs(distanceToVertex.z);
				distanceToVertex.z = 0;

				// Checks if the vertex is near the ray using the angle between rays
				float angle = Vector3.Angle(distanceToVertex, _rayDirections[i]) * Mathf.Deg2Rad;
				if (Mathf.Abs(angle) <= angleBetweenRays) {
					// Calculates the weight of the vertex
					float zFactor = 1 - zDistance / sphereCollider.radius;
					_deformationWeights[i][j] = Mathf.Sqrt((1 - angle / angleBetweenRays) * zFactor);
				}
				else
					_deformationWeights[i][j] = 0;
			}

            // Calculates the deformation weights of the ray for the extra points
            Vector3[] localExtraPointsPosition = GetLocalExtraPointsPosition();
			_extraPointsWeights[i] = new float[localExtraPointsPosition.Length];
			for (int j = 0; j < localExtraPointsPosition.Length; j++) {
				// Calculates the distance from the center to the point in local coordinates
				Vector3 distanceToPoint = localExtraPointsPosition[j] - center;

				// Isolates the Z coordinate, as the rays are not casted in that direction
				float zDistance = Mathf.Abs(distanceToPoint.z);
				distanceToPoint.z = 0;

				// Checks if the point is near the ray using the angle between rays
				float angle = Vector3.Angle(distanceToPoint, _rayDirections[i]) * Mathf.Deg2Rad;
				if (Mathf.Abs(angle) <= angleBetweenRays) {
					// Calculates the weight of the point
					float zFactor = 1 - zDistance / sphereCollider.radius;
					_extraPointsWeights[i][j] = Mathf.Sqrt((1 - angle / angleBetweenRays) * zFactor);
				}
				else
					_extraPointsWeights[i][j] = 0;
			}
		}
	}

    /// <summary>
    /// Returns an array with the local positions on the current frame of each
    /// extra point.
    /// </summary>
    /// <returns>Local positions of the extra points</returns>
    private Vector3[] GetLocalExtraPointsPosition() {
        Vector3[] localExtraPointsPosition = new Vector3[_originalExtraPointsPositions.Length];
        for (int i = 0; i < _originalExtraPointsPositions.Length; i++)
            localExtraPointsPosition[i] = _transform.InverseTransformPoint(extraDeformationPoints[i].parent.TransformPoint(_originalExtraPointsPositions[i]));
        return localExtraPointsPosition;
    }
    
	/// <summary>
	/// Casts the rays and starts the deformation.
	/// </summary>
	private void Deform() {
		// Initializes the arrays
		Vector3[] modifiedVertices = (Vector3[])_originalVertices.Clone();
        Vector3[] localExtraPointsPositions = GetLocalExtraPointsPosition();
        Vector3[] extraPoints = (Vector3[])localExtraPointsPositions.Clone();

        // Calculates the needed information
        Vector3 center = _transform.TransformPoint(sphereCollider.center);
		float rayDistance = sphereCollider.radius * _transform.lossyScale.x;

		// Casts the rays
		Vector3[] deformations = new Vector3[numberOfRays];
		for (int i = 0; i < numberOfRays; i++) {
			// If the ray didn't hit anything, skips it
			RaycastHit hitInfo;
			Vector3 transformedDirection = _rayDirections[i];   // Patch: No modifications are done to the direction
			if (!Physics.Raycast(center, transformedDirection, out hitInfo, rayDistance, layer)) {
				deformations[i] = Vector3.zero;
				continue;
			}
			else
				Debug.DrawRay(center, transformedDirection * rayDistance, Color.red);

			// Calculates the deformation
			deformations[i] = transformedDirection * (hitInfo.distance - rayDistance);
			deformations[i] = Vector3.Project(deformations[i], hitInfo.normal);

			// For each vertex, calculates the chamf deformation to apply
			DeformChamfVertices(i, deformations[i], _originalVertices, ref modifiedVertices);

			// Deforms the extra points as well
			DeformChamfVertices(i, deformations[i], localExtraPointsPositions, ref extraPoints);
		}

		// For each vertex, calculates the planar deformation to apply
		for (int i = 0; i < numberOfRays; i++)
			if (deformations[i] != Vector3.zero) {
				DeformPlanarVertices(i, deformations[i], _originalVertices, ref modifiedVertices, _deformationWeights);

				// Deforms the extra points as well
				DeformPlanarVertices(i, deformations[i], localExtraPointsPositions, ref extraPoints, _extraPointsWeights);
			}

		// Moves the vertices to their desired position
		Vector3[] newVertices = new Vector3[modifiedVertices.Length];
		Vector3[] lastFrameVertices = _modifiedMesh.vertices;
		for (int i = 0; i < newVertices.Length; i++)
			newVertices[i] = Vector3.Lerp(lastFrameVertices[i], modifiedVertices[i], deformationSpeed * Time.deltaTime);

		// Moves the extra points to their desired position
		for (int i = 0; i < extraDeformationPoints.Length; i++)
			extraDeformationPoints[i].position = _transform.TransformPoint(Vector3.Lerp(_transform.InverseTransformPoint(extraDeformationPoints[i].position), extraPoints[i], deformationSpeed * Time.deltaTime));

		// Reassignates the vertices and recalculates the normals of the vertices
		_modifiedMesh.vertices = newVertices;
		//_modifiedMesh.RecalculateNormals();   // These may break the model
		//_modifiedMesh.RecalculateBounds();
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
		float deformationFactor = deformation.magnitude * chamfScale;
		deformationFactor /= sphereCollider.radius * _transform.lossyScale.x * Mathf.Sqrt(numberOfRays);

		// For each vertex, calculates it's deformation
		for (int i = 0; i < vertexToModify.Length; i++) {
			// Calculates the distance to the deformation point
			Vector3 vertexDistance = _transform.TransformVector(originalVertices[i] - _deformationPoints[rayIndex]);
			Vector3 distanceProjection = Vector3.ProjectOnPlane(vertexDistance, deformation);

			// Applys the chamf to the vertex
			vertexToModify[i] += _transform.InverseTransformVector(distanceProjection * deformationFactor);
		}
	}

	/// <summary>
	/// Calculates the deformation on the deformation direction derived
	/// from a single ray, compensating the chamf.
	/// </summary>
	/// <param name="rayIndex">Index of the ray causing the deformation</param>
	/// <param name="deformation">Deformation performed by the ray</param>
	/// <param name="vertexToModify">A reference to the vertices to modify</param>
	private void DeformPlanarVertices(int rayIndex, Vector3 deformation, Vector3[] originalVertices, ref Vector3[] vertexToModify, float[][] deformationWeigths) {
		// For each vertex, calculates it's deformation
		for (int i = 0; i < vertexToModify.Length; i++) {
			// Calculates the chamf compensation
			Vector3 chamfDistance = Vector3.zero;
			if (compensateChamf) {
				chamfDistance = _transform.TransformVector(originalVertices[i] - vertexToModify[i]);
				chamfDistance = Vector3.Project(chamfDistance, deformation);
			}

			// Adds the deformation to the vertex
			vertexToModify[i] += _transform.InverseTransformVector(deformation + chamfDistance) * deformationWeigths[rayIndex][i];
		}
	}

	#endregion
}
