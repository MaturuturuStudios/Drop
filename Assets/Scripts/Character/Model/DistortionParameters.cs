using UnityEngine;
using System;

[Serializable]
public class DistortionParameters {
	[Range(0, 128)]
	public float distortionAmount = 25.0f;

	[Range(0, 128)]
	public float distortionSpeed = 0.25f;
	
	public Vector2 waveAmplitude;
	public Vector2 waveSpeed;
	public Vector2 waveFrequency;

	public static DistortionParameters GetInterpolatedParameters(DistortionParameters minParameters, DistortionParameters maxParameters, float factor) {
		DistortionParameters parameters = new DistortionParameters();

		parameters.distortionAmount = Mathf.Lerp(minParameters.distortionAmount, maxParameters.distortionAmount, factor);
		parameters.distortionSpeed = Mathf.Lerp(minParameters.distortionSpeed, maxParameters.distortionSpeed, factor);
		parameters.waveAmplitude = Vector2.Lerp(minParameters.waveAmplitude, maxParameters.waveAmplitude, factor);
		parameters.waveSpeed = Vector2.Lerp(minParameters.waveSpeed, maxParameters.waveSpeed, factor);
		parameters.waveFrequency = Vector2.Lerp(minParameters.waveFrequency, maxParameters.waveFrequency, factor);

		return parameters;
	}
}