using UnityEngine;

public static class RaycastExtension {

	public static bool RaycastMesh(Mesh mesh, Vector3 origin, Vector3 direction, out RaycastHit hit) {
		RaycastHit temp;
		bool value = RaycastMesh(mesh, new Ray(origin, direction), out temp);
		hit = temp;
		return value;
	}

	public static bool RaycastMesh(Mesh mesh, Ray ray, out RaycastHit hit) {
		// Checks if the ray hits any triangle of the mesh
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		bool value = false;
		hit = new RaycastHit();
		for (int i = 0; i < triangles.Length; i += 3) {
			Triangle triangle = new Triangle(vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]);
			if (RaycastTriangle(triangle, ray, out hit)) {
				value = true;
				break;
			}
		}
		return value;
	}

	public static bool RaycastTriangle(Triangle triangle, Vector3 origin, Vector3 direction, out RaycastHit hit) {
		RaycastHit temp;
		bool value = RaycastTriangle(triangle, new Ray(origin, direction), out temp);
		hit = temp;
		return value;
	}

	public static bool RaycastTriangle(Triangle triangle, Ray ray, out RaycastHit hit) {
		// Checks if the ray hits the plane defined by the triangle
		hit = new RaycastHit();
		Plane trianglePlane = triangle.GetPlane();
		float planeDistance;
		if (!trianglePlane.Raycast(ray, out planeDistance))
			return false;

		// Gets the point's projection on the plane
		Vector3 point = ray.GetPoint(planeDistance);

		// Checks if the triangle contains the point
		float u, v;
		triangle.GetBaricentricCoordinates(point, out u, out v);
		if (!triangle.Contains(u, v))
			return false;

		// Populates the raycast hit information
		hit.normal = triangle.GetPerpendicular();
		hit.point = point;
		return true;
	}
}

public class Triangle {

	public Vector3[] vertices;

	public Triangle(Vector3[] points) {
		if (points.Length != 3) {
			Debug.LogError("Error: Invalid number of vertices for a traingle: " + points.Length);
			return;
		}
		vertices = points;
	}

	public Triangle(Vector3 p0, Vector3 p1, Vector3 p2) {
		vertices = new Vector3[3];
		vertices[0] = p0;
		vertices[1] = p1;
		vertices[2] = p2;
	}

	public Vector3 GetNormal() {
		return GetPerpendicular().normalized;
	}

	public Vector3 GetPerpendicular() {
		return Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]);
	}

	public Plane GetPlane() {
		return new Plane(vertices[0], vertices[1], vertices[2]);
	}

	public void GetBaricentricCoordinates(Vector3 point, out float u, out float v) {
		// Compute vectors
		Vector3 v0 = vertices[2] - vertices[0];
		Vector3 v1 = vertices[1] - vertices[0];
		Vector3 v2 = point - vertices[0];

		// Compute dot products
		float dot00 = Vector3.Dot(v0, v0);
		float dot01 = Vector3.Dot(v0, v1);
		float dot02 = Vector3.Dot(v0, v2);
		float dot11 = Vector3.Dot(v1, v1);
		float dot12 = Vector3.Dot(v1, v2);

		// Compute barycentric coordinates
		float denom = dot00 * dot11 - dot01 * dot01;
		u = (dot11 * dot02 - dot01 * dot12) / denom;
		v = (dot00 * dot12 - dot01 * dot02) * denom;
	}

	public bool Contains(Vector3 point) {
		float u, v;
		GetBaricentricCoordinates(point, out u, out v);
		return Contains(u, v);
	}

	public bool Contains(float u, float v) {
		// Check if point is in triangle
		return (u >= 0) && (v >= 0) && (u + v < 1);
	}
}
