using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WaterEffects : MonoBehaviour, WaterRepulsionListener {

    /// <summary>
    /// Effect of waves
    /// </summary>
    public GameObject waves;

	/// <summary>
	/// Effects for sizes above 2
	/// </summary>
	public GameObject splash;

	/// <summary>
	/// Effects for sizes above 4
	/// </summary>
	public GameObject columnWater;
    /// <summary>
    /// Duration of the effect
    /// </summary>
	public float duration = 5.0f;
    /// <summary>
    /// Offset of the effect in y axis
    /// </summary>
    public float yOffset = 0f;

	// Use this for initialization
	void Start () {
        GetComponentInChildren<WaterRepulsion>().AddListener(this);
    }

    public void OnWaterEnter(WaterRepulsion water, GameObject character) {
        PutEffect(water, character, false);
    }

    public void OnWaterExit(WaterRepulsion water, GameObject character, Vector3 repulsionVelocity) {
        PutEffect(water, character, true);
    }

    private void PutEffect(WaterRepulsion water, GameObject character, bool exit) {
        CharacterSize sizeCharacter = character.GetComponent<CharacterSize>();
        int size=sizeCharacter.GetSize();
        Bounds collider = water.GetCollider();
        Vector3 position = character.transform.position;
        position.y = collider.max.y + 0.2f + yOffset;
        position.z = 0;

        List<GameObject> effects = new List<GameObject>();

        if (size > 0) {
            GameObject effect = Instantiate(waves, position, Quaternion.identity) as GameObject;

            ParticleSystem[] systems = effect.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem sys in systems) {
                sys.randomSeed = (uint)UnityEngine.Random.Range(0, int.MaxValue);
                sys.Simulate(0, true, true);
                sys.Play();
            }

            effect.transform.localScale = size * Vector3.one;
			effects.Add(effect);
        }

        if (size > 2) {
            GameObject effect = Instantiate(splash, position, Quaternion.identity) as GameObject;

            ParticleSystem[] systems = effect.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem sys in systems) {
                sys.randomSeed = (uint)UnityEngine.Random.Range(0, int.MaxValue);
                sys.Simulate(0, true, true);
                sys.Play();
            }

            effect.transform.localScale = size * Vector3.one;
			effects.Add(effect);
        }

        if (size > 4 && !exit){
            GameObject effect = Instantiate(columnWater, position, Quaternion.identity) as GameObject;

            ParticleSystem[] systems = effect.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem sys in systems) {
                sys.randomSeed = (uint)UnityEngine.Random.Range(0, int.MaxValue);
                sys.Simulate(0, true, true);
                sys.Play();
            }

            effect.transform.localScale = size * Vector3.one;
            effects.Add(effect);
        }

        // Destroys the systems
        foreach (GameObject effect in effects) {
			GameControllerTemporal.AddTemporal(effect);
			Destroy(effect, duration);
        }
    }
}
