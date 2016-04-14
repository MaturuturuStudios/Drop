using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Class used by triggers to invoke methods on
/// different objects. Fully configurable from the
/// editor.
/// </summary>
[Serializable]
public class MethodInvoke {
	
	#region Fields

	/// <summary>
	/// The object whose method will be invoked.
	/// </summary>
	public GameObject target;

	/// <summary>
	/// From all the methods of the target, the index of
	/// the selected one.
	/// </summary>
	public int selectedIndex;

	/// <summary>
	/// Parameter used for integer numbers.
	/// </summary>
	public Int32 Int32Parameter;

	/// <summary>
	/// Parameter used for long integer numbers.
	/// </summary>
	public Int64 Int64Parameter;

	/// <summary>
	/// Parameter used for floating point numbers.
	/// </summary>
	public Single SingleParameter;

	/// <summary>
	/// Parameter used for double precision floating point numbers.
	/// </summary>
	public Double DoubleParameter;

	/// <summary>
	/// Parameter used for boolean values.
	/// </summary>
	public Boolean BooleanParameter;

	/// <summary>
	/// Parameter used for character strings.
	/// </summary>
	public String StringParameter;

	/// <summary>
	/// Parameter used for RGBA colors.
	/// </summary>
	public Color ColorParameter;

	/// <summary>
	/// Parameter used for rectangles.
	/// </summary>
	public Rect RectParameter;

	/// <summary>
	/// Parameter used for bounds.
	/// </summary>
	public Bounds BoundsParameter;

	/// <summary>
	/// Parameter used for two dimensional vectors.
	/// </summary>
	public Vector2 Vector2Parameter;

	/// <summary>
	/// Parameter used for three dimensional vectors.
	/// </summary>
	public Vector3 Vector3Parameter;

	/// <summary>
	/// Parameter used for four dimensional vectors.
	/// </summary>
	public Vector4 Vector4Parameter;

	/// <summary>
	/// Parameter used for animation curves.
	/// </summary>
	public AnimationCurve AnimationCurveParameter;

	/// <summary>
	/// Parameter used for unity's objects.
	/// </summary>
	public UnityEngine.Object ObjectParameter;

	#endregion

	#region Methods

	/// <summary>
	/// Invokes the selected method.
	/// </summary>
	public void Invoke() {
		if (target == null)
			Debug.LogError("Error: No selected target for the invocation.");

		MethodInfo selectedMethod = GetMethodsInfo(target)[selectedIndex];
		ParameterInfo[] parameters = selectedMethod.GetParameters();
		if (parameters.Length == 0)
			target.SendMessage(selectedMethod.Name, SendMessageOptions.RequireReceiver);
		else if (parameters.Length == 1)
			target.SendMessage(selectedMethod.Name, GetParameterOfType(parameters[0].ParameterType), SendMessageOptions.RequireReceiver);
		else
			Debug.LogError("Error: The invoked method has more than one paramter: " + selectedMethod.Name);
	}

	/// <summary>
	/// Returns the right parameter of the given type.
	/// </summary>
	/// <param name="type">The type of the parameter to find</param>
	/// <returns>The parameter of the given type</returns>
	public object GetParameterOfType(Type type) {
		if (type == typeof(Int32))
			return Int32Parameter;
		else if (type == typeof(Int64))
			return Int64Parameter;
		else if (type == typeof(Single))
			return SingleParameter;
		else if (type == typeof(Double))
			return DoubleParameter;
		else if (type == typeof(Boolean))
			return BooleanParameter;
		else if (type == typeof(String))
			return StringParameter;
		else if (type == typeof(Color))
			return ColorParameter;
		else if (type == typeof(Rect))
			return RectParameter;
		else if (type == typeof(Bounds))
			return BoundsParameter;
		else if (type == typeof(Vector2))
			return Vector2Parameter;
		else if (type == typeof(Vector3))
			return Vector3Parameter;
		else if (type == typeof(Vector4))
			return Vector4Parameter;
		else if (type == typeof(AnimationCurve))
			return AnimationCurveParameter;
		else if (type == typeof(UnityEngine.Object))
			return ObjectParameter;
		else {
			Debug.LogError("Error: Unsupported parameter type: " + type.Name);
			return null;
		}
	}

	/// <summary>
	/// Gives the information of every method of a GameObject.
	/// </summary>
	/// <param name="target">The GameObject</param>
	/// <returns>The information of all methods of the object</returns>
	public static List<MethodInfo> GetMethodsInfo(GameObject target) {
		if (target == null)
			return null;

		List<MethodInfo> methods = new List<MethodInfo>();
		Component[] objectComponents = target.GetComponents<Component>();
		foreach (Component component in objectComponents) {
			MethodInfo[] methodInfo = component.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			methods.AddRange(methodInfo);
		}

		return methods;
	}

	#endregion
}

/// <summary>
/// Auxiliar class for a list of MethodInvoke. Required for
/// the ReorderableList drawer to work properly.
/// </summary>
[Serializable]
public class ReorderableList_MethodInvoke : ReorderableList<MethodInvoke> {
	
}
