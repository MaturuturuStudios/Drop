using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class MethodInvoke {

	public GameObject target;
	public int selectedIndex;

	public Int32 Int32Parameter;
	public Int64 Int64Parameter;
	public Single SingleParameter;
	public Double DoubleParameter;
	public Boolean BooleanParameter;
	public String StringParameter;
	public Color ColorParameter;
	public Rect RectParameter;
	public Bounds BoundsParameter;
	public Vector2 Vector2Parameter;
	public Vector3 Vector3Parameter;
	public Vector4 Vector4Parameter;
	public AnimationCurve AnimationCurveParameter;
	public UnityEngine.Object ObjectParameter;

	public void Invoke() {
		MethodInfo selectedMethod = GetMethodsInfo(target)[selectedIndex];
		ParameterInfo[] parameters = selectedMethod.GetParameters();
		if (parameters.Length == 0)
			target.SendMessage(selectedMethod.Name, SendMessageOptions.RequireReceiver);
		else if (parameters.Length == 1)
			target.SendMessage(selectedMethod.Name, GetParameterOfType(parameters[0].ParameterType), SendMessageOptions.RequireReceiver);
		else
			Debug.LogError("Error: The invoked method has more than one paramter: " + selectedMethod.Name);
	}

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
}

[Serializable]
public class ReorderableList_MethodInvoke : ReorderableList<MethodInvoke> {
	
}
