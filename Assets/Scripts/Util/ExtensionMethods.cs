using UnityEngine;

/// <summary>
/// Special class to implement extension methods for
/// existing classes.
/// </summary>
public static class ExtensionMethods {

	#region Mesh

	/// <summary>
	/// Casts a ray into the mesh.
	/// </summary>
	/// <param name="mesh">This mesh</param>
	/// <param name="ray">The ray casted</param>
	/// <param name="hit">Information about the hit</param>
	/// <returns>If the ray hit the mesh</returns>
	public static bool Raycast(this Mesh mesh, Ray ray, out RaycastHit hit) {
		return Raycast(mesh, ray.origin, ray.direction, out hit);
	}

	/// <summary>
	/// Casts a ray into the mesh.
	/// </summary>
	/// <param name="mesh">This mesh</param>
	/// <param name="origin">Origin of the ray</param>
	/// <param name="direction">Direction of the ray</param>
	/// <param name="hit">Information about the hit</param>
	/// <returns>If the ray hit the mesh</returns>
	public static bool Raycast(this Mesh mesh, Vector3 origin, Vector3 direction, out RaycastHit hit) {
		// Initializes the mesh information
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;

		// Initializes the values needed for the raycast
		hit = new RaycastHit();
		float closestDistance = float.MaxValue;

		// For each triangle, checks the raycast
		for (int i = 0; i < triangles.Length; i += 3) {
			Vector3 v0 = vertices[triangles[i]];
			Vector3 v1 = vertices[triangles[i + 1]];
			Vector3 v2 = vertices[triangles[i + 2]];

			// Casts the ray into the triangle
			RaycastHit triangleHit;
            if (RaycastTriangle(origin, direction, v0, v1, v2, out triangleHit)) {
				// Checks if the triangle hit is the nearest one so far
				if (triangleHit.distance < closestDistance) {
					hit = triangleHit;
					closestDistance = hit.distance;
				}
			}
		}

		// If no triangle was hit, the distance was not modified
		return closestDistance < float.MaxValue;
	}

	#endregion

	#region Util

	/// <summary>
	/// Checks if a ray hits a triangle.
	/// </summary>
	/// <param name="origin">Origin of the ray</param>
	/// <param name="direction">Direction of the ray</param>
	/// <param name="v0">First vertex of the triangle (clockwise order)</param>
	/// <param name="v1">Second vertex of the triangle (clockwise order)</param>
	/// <param name="v2">Third vertex of the triangle (clockwise order)</param>
	/// <param name="hit">Information about the hit</param>
	/// <returns>If the ray hit the triangle</returns>
	public static bool RaycastTriangle(Vector3 origin, Vector3 direction, Vector3 v0, Vector3 v1, Vector3 v2, out RaycastHit hit) {
		// Initializes the hit information
		hit = new RaycastHit();

		Vector3 edge1 = v1 - v0;
		Vector3 edge2 = v2 - v0;

		// There is no hit if the ray and the normal are perpendicular
		Vector3 pVec = Vector3.Cross(direction, edge2);
		float det = Vector3.Dot(edge1, pVec);
		if (det < 0.0001) // Close to 0 
			return false;

		// Checks the baricentric coordinates
		Vector3 tVec = origin - v0;
		float u = Vector3.Dot(tVec, pVec);
		if (u < 0 || u > det)
			return false;

		Vector3 qVec = Vector3.Cross(tVec, edge1);
		float v = Vector3.Dot(direction, qVec);
		if (v < 0 || u + v > det)
			return false;

		// At this point, the ray hits the triangle. Fills the hit information
		hit.distance = Vector3.Dot(edge2, qVec) / det;
		hit.point = origin + direction * hit.distance;
		hit.normal = Vector3.Cross(edge1, edge2);
		return true;
	}

	#endregion
}
