using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class MethodInvoke {

	public GameObject target;
	public int selectedIndex;

	public void Invoke() {
		target.SendMessage(GetMethodsInfo(target)[selectedIndex].Name, SendMessageOptions.RequireReceiver);
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
