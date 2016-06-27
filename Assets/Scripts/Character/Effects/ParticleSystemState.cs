using UnityEngine;

/// <summary>
/// Saves the state of a particle system to modify it at runtime.
/// </summary>
public class ParticleSystemState {

	/// <summary>
	/// Reference to the particle system which state is saved.
	/// </summary>
	public ParticleSystem system;

	/// <summary>
	/// Initial start size of the particle system.
	/// </summary>
	public float startSize;

	/// <summary>
	/// Initial start speed of the particle system.
	/// </summary>
	public float startSpeed;

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="system">The system which state to save</param>
	public ParticleSystemState(ParticleSystem system) {
		// Saves the particle system's initial state
		this.system = system;
		startSize = system.startSize;
		startSpeed = system.startSpeed;
	}

	public void UpdateWithSize(float sizeFactor) {
		// Scales the start size and speed
		system.startSize = startSize * sizeFactor;
		system.startSpeed = startSpeed * sizeFactor;

		// Scales the VelocityOverLifetime module as well
		if (system.velocityOverLifetime.enabled) {
			ParticleSystem.MinMaxCurve x = system.velocityOverLifetime.x;
			x.curveScalar = sizeFactor;
			ParticleSystem.MinMaxCurve y = system.velocityOverLifetime.x;
			y.curveScalar = sizeFactor;
			ParticleSystem.MinMaxCurve z = system.velocityOverLifetime.x;
			z.curveScalar = sizeFactor;
		}
	}
}
