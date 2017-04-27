using UnityEngine;
using System;

[Serializable]
public class DeformationParameters {

	public float adaptationAcceleration = 30.0f;

	public float adaptationDamping = 2.0f;

	public float tensionForce = 15.0f;

	[Range(0, 1)]
	public float inhertiaScale = 0.5f;

	public float modelScale = 1.0f;

	public static DeformationParameters GetInterpolatedParameters(DeformationParameters minParameters, DeformationParameters maxParameters, float factor) {
		DeformationParameters parameters = new DeformationParameters();

		parameters.adaptationAcceleration = Mathf.Lerp(minParameters.adaptationAcceleration, maxParameters.adaptationAcceleration, factor);
		parameters.adaptationDamping = Mathf.Lerp(minParameters.adaptationDamping, maxParameters.adaptationDamping, factor);
		parameters.tensionForce = Mathf.Lerp(minParameters.tensionForce, maxParameters.tensionForce, factor);
		parameters.inhertiaScale = Mathf.Lerp(minParameters.inhertiaScale, maxParameters.inhertiaScale, factor);
		parameters.modelScale = Mathf.Lerp(minParameters.modelScale, maxParameters.modelScale, factor);

		return parameters;
	}
}