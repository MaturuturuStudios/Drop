using UnityEngine;
using System;

[ExecuteInEditMode]
public class SphereDeformationShader : MonoBehaviour {

	private static readonly float maxDistanceFactor = 2.0f;
	private static readonly bool debugRays = false;

	public LayerMask deformationMask;

	[Range(2, 64)]
	[SerializeField]
	private int numberOfSections = 16;

	[Range(1, 10)]
	public int numberOfIterations = 2;

	public float deformationDistance = 0.5f;

	//[SerializeField]
	private DeformationParameters deformationParameters;

	//[SerializeField]
	private DistortionParameters distortionParameters;

	private bool[] raysHits;
	private float[] raysDistances;
	private float[] raysSpread;

	private float[] raysCurrentDistance;
	private float[] raysCurrentSpeed;

	private Vector3 lastFramePosition;

	private Vector3[] raysDirections;

	private Transform _transform;
	private Renderer _renderer;

	void Awake() {
		_transform = transform;
		_renderer = GetComponent<Renderer>();

		SetNumberOfSections(numberOfSections);
	}

	void OnEnable() {
		lastFramePosition = _transform.position;
	}

	public void SetNumberOfSections(int amount) {
		numberOfSections = amount;

		raysDirections = new Vector3[numberOfSections];

		raysHits = new bool[numberOfSections];
		raysDistances = new float[numberOfSections];
		raysSpread = new float[numberOfSections];

		raysCurrentDistance = new float[numberOfSections];
		raysCurrentSpeed = new float[numberOfSections];

		for (int i = 0; i < numberOfSections; ++i) {
			float angle = 2.0f * Mathf.PI * i / numberOfSections;
			raysDirections[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
		}

		RestartDeformation();
	}

	public void RestartDeformation() {
		for (int i = 0; i < numberOfSections; ++i) {
			raysHits[i] = false;
			raysDistances[i] = GetDeformationDistance();
			raysSpread[i] = 0;

			raysCurrentDistance[i] = raysDistances[i];
			raysCurrentSpeed[i] = 0;
		}
	}

	public void SetDeformationParameters(DeformationParameters parameters) {
		deformationParameters = parameters;
		RestartDeformation();
	}

	public void SetDistortionParameters(DistortionParameters parameters) {
		distortionParameters = parameters;
	}

	private float GetDeformationDistance() {
		return deformationDistance * _transform.lossyScale.x;
	}

	void LateUpdate() {

		if (Application.isEditor && raysDirections.Length != numberOfSections)
			SetNumberOfSections(numberOfSections);

		if (Application.isPlaying) {

			ResetRaysValues();

			for (int iteration = 0; iteration < numberOfIterations; ++iteration) {

				CastRays();
			}

			if (Application.isEditor && debugRays)
				DrawRays();
		}

		PassDataToShader();
	}

	void FixedUpdate() {

		if (Application.isPlaying) {
			AddInhertia();

			UpdateRays();

			ApplyTension();

			ClampDistances();
		}

	}

	private void AddInhertia() {

		Vector3 inhertia = (_transform.position - lastFramePosition) * deformationParameters.inhertiaScale;

		for (int ray = 0; ray < numberOfSections; ++ray) {

			Vector3 rayInhertia = Vector3.Project(inhertia, raysDirections[ray]);
			float inhertiaAmount = rayInhertia.magnitude;
			if (Vector3.Dot(rayInhertia, raysDirections[ray]) < 0)
				inhertiaAmount *= -1;
			raysCurrentSpeed[ray] -= inhertiaAmount;
		}

		lastFramePosition = _transform.position;
	}

	private void UpdateRays() {
		for (int ray = 0; ray < numberOfSections; ++ray) {

			float acceleration = (ComputeRayDistance(ray) - raysCurrentDistance[ray]) * deformationParameters.adaptationAcceleration;
			raysCurrentSpeed[ray] += acceleration * Time.deltaTime;

			raysCurrentSpeed[ray] *= Mathf.Clamp(1 - deformationParameters.adaptationDamping * Time.deltaTime, 0, 1);

			raysCurrentDistance[ray] += raysCurrentSpeed[ray] * Time.deltaTime;
		}
	}

	private void ApplyTension() {

		float force = deformationParameters.adaptationAcceleration * deformationParameters.tensionForce / 2.0f;

		for (int ray = 0; ray < numberOfSections; ++ray) {

			float tensionAmount = 0;

			int index = (ray + 1) % numberOfSections;
			tensionAmount += (raysCurrentDistance[index] - raysCurrentDistance[ray]);

			index = ray - 1;
			if (index < 0)
				index += numberOfSections;
			tensionAmount += (raysCurrentDistance[index] - raysCurrentDistance[ray]);

			raysCurrentSpeed[ray] += force * tensionAmount * Time.deltaTime;
		}
	}

	private void ResetRaysValues() {
		for (int ray = 0; ray < numberOfSections; ++ray) {

			raysHits[ray] = false;

			raysDistances[ray] = GetDeformationDistance();

			raysSpread[ray] = 0;
		}
	}

	private void CastRays() {
		float[] newSpreads = new float[numberOfSections];

		for (int ray = 0; ray < numberOfSections; ++ray) {

			RaycastHit hit;
			raysHits[ray] = Physics.Raycast(_transform.position, raysDirections[ray], out hit, raysCurrentDistance[ray], deformationMask);

			if (raysHits[ray]) {

				float spread = (GetDeformationDistance() - hit.distance) / (numberOfSections - 1);

				for (int i = 0; i < numberOfSections; i++) {
					if (i != ray) {
						newSpreads[i] += spread;
					}
				}

				raysCurrentDistance[ray] = hit.distance;
				if (raysCurrentSpeed[ray] > 0) {
					raysCurrentSpeed[ray] = 0;
				}

				raysDistances[ray] =  Mathf.Min(hit.distance, GetDeformationDistance() + raysSpread[ray]);
			}
			else {

				raysDistances[ray] = GetDeformationDistance();
			}
		}
		
		for (int ray = 0; ray < numberOfSections; ++ray) {

			if (!raysHits[ray]) {

				raysSpread[ray] = newSpreads[ray];
			}
		}
	}

	private void DrawRays() {
		for (int ray = 0; ray < numberOfSections; ++ray) {
			
			Debug.DrawRay(_transform.position, raysDirections[ray] * raysCurrentDistance[ray], Color.red);
		}
	}

	private float ComputeRayDistance(int rayIndex) {
		if (!raysHits[rayIndex]) {
			return raysDistances[rayIndex] + raysSpread[rayIndex];
		}
		else {
			return raysDistances[rayIndex];
		}
	}

	private void ClampDistances() {
		float maxDistance = maxDistanceFactor * GetDeformationDistance();
		for (int ray = 0; ray < numberOfSections; ++ray) {
			if (raysCurrentDistance[ray] > maxDistance) {
				raysCurrentSpeed[ray] = 0;
				raysCurrentDistance[ray] = maxDistance;
			}
			else if (raysCurrentDistance[ray] < 0) {
				raysCurrentSpeed[ray] = 0;
				raysCurrentDistance[ray] = 0;
			}
		}
	}

	private void PassDataToShader() {

		float[] offsets = new float[numberOfSections];
		Vector4[] directions = new Vector4[numberOfSections];
		for (int i = 0; i < numberOfSections; ++i) {
			offsets[i] = raysCurrentDistance[i] - GetDeformationDistance();
			directions[i] = new Vector4(raysDirections[i].x, raysDirections[i].y, raysDirections[i].z);
		}

		MaterialPropertyBlock deformationInfo = new MaterialPropertyBlock();
		
		deformationInfo.SetFloat("DeformationDistance", GetDeformationDistance());
		deformationInfo.SetFloat("DeformationRays", numberOfSections);
		deformationInfo.SetVector("DeformationCenter", _transform.position);
		deformationInfo.SetFloatArray("DeformationOffsets", offsets);
		deformationInfo.SetVectorArray("DeformationDirections", directions);
		
		if (distortionParameters != null) {
			deformationInfo.SetFloat("_Distort", distortionParameters.distortionAmount);
			deformationInfo.SetFloat("_DistortSpeed", distortionParameters.distortionSpeed);
			deformationInfo.SetFloat("_WaveAmpX", distortionParameters.waveAmplitude.x);
			deformationInfo.SetFloat("_WaveAmpY", distortionParameters.waveAmplitude.y);
			deformationInfo.SetFloat("_WaveSpeedX", distortionParameters.waveSpeed.x);
			deformationInfo.SetFloat("_WaveSpeedY", distortionParameters.waveSpeed.y);
			deformationInfo.SetFloat("_WaveFreqX", distortionParameters.waveFrequency.x);
			deformationInfo.SetFloat("_WaveFreqY", distortionParameters.waveFrequency.y);
		}

		_renderer.SetPropertyBlock(deformationInfo);
	}

	private void OnDrawGizmosSelected() {
		Transform trf = transform;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(trf.position, deformationDistance * trf.lossyScale.x);
		for (int i = 0; i < numberOfSections; ++i) {
			float angle = 2.0f * Mathf.PI * i / numberOfSections;
			Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
			Gizmos.DrawRay(trf.position, direction * deformationDistance * trf.lossyScale.x);
		}
	}
}
