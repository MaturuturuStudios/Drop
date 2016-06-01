using UnityEngine;
using System.Collections;

/// <summary>
/// Fires events when a certain object is irrigated.
/// It's fully configurable on the editor.
/// </summary>
public class TriggerIrrigate : Irrigate {

	#region Public Attributes

	/// <summary>
	/// If enabled, the Gizmos will be drawn in the editor even
	/// if the entity is not selected.
	/// </summary>
	public bool drawGizmos = false;

	/// <summary>
	/// Events fired when the trigger is activated.
	/// </summary>
	public ReorderableList_MethodInvoke onIrrigate = new ReorderableList_MethodInvoke();

    #endregion

    #region Methods


    /// <summary>
    /// TODO: variable temporal a borrar luego de presentacion
    /// </summary>
    public ParticleSystem particleGrow;
    public GameObject temporalObject;
    /// <summary>
    /// TODO: método temporal a borrar
    /// </summary>
    public void EnableObject() {
        //set the particles
        GameObject particleSystem = Instantiate(particleGrow.gameObject) as GameObject;
        Vector3 position = transform.position;
        particleSystem.GetComponent<Transform>().position = position;
        //destroy system
        Destroy(particleSystem, particleGrow.startLifetime*2);
        //grow the plant
        StartCoroutine(EnableTheObject());
    }
    private IEnumerator EnableTheObject() {
        yield return new WaitForSeconds(3);
        temporalObject.SetActive(true);
    }

    /// <summary>
    /// Fires the events when the object is irrigated.
    /// </summary>
    protected override void OnIrrigate() {
		// Performs the method invocations
		foreach (MethodInvoke methodInvoke in onIrrigate.AsList())
			methodInvoke.Invoke();
	}

	/// <summary>
	/// Unity's method called on the editor to draw helpers.
	/// </summary>
	public void OnDrawGizmos() {
		if (drawGizmos)
			OnDrawGizmosSelected();
	}

	/// <summary>
	/// Unity's method called on the editor to draw helpers only
	/// while the object is selected.
	/// </summary>
	public void OnDrawGizmosSelected() {		
		Gizmos.matrix = Matrix4x4.identity;
		Gizmos.color = Color.green;
		foreach (MethodInvoke methodInvoke in onIrrigate.AsList())
			DrawMethodInvoke(methodInvoke);

		Gizmos.matrix = transform.localToWorldMatrix;
		Color colliderColor = Color.cyan;
		colliderColor.a = 0.5f;
		Gizmos.color = colliderColor;
	}

	/// <summary>
	/// Draws a line to the target of a method invoke.
	/// Also draws a rect if it's parameter is one.
	/// </summary>
	/// <param name="methodInvoke">The method invoke to draw</param>
	/// <param name="separation">The separation of the line</param>
	private void DrawMethodInvoke(MethodInvoke methodInvoke, Vector3 separation = new Vector3()) {
		if (methodInvoke.target != null) {
			Gizmos.DrawLine(transform.position + separation, methodInvoke.target.transform.position + separation);
			Color temp = Gizmos.color;
			Color newColor = Color.yellow;
			newColor.a = 0.25f;
			Gizmos.color = newColor;
			Gizmos.DrawCube(methodInvoke.RectParameter.center, methodInvoke.RectParameter.size);
			Gizmos.color = temp;
		}
	}

	#endregion
}