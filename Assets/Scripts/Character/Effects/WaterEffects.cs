using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WaterEffects : MonoBehaviour, WaterRepulsionListener {
    public GameObject waves;
    public GameObject columnWater;
    public GameObject splash;

    private float time = 5;

	// Use this for initialization
	void Start () {
        WaterRepulsion[] waters = GetComponentsInChildren<WaterRepulsion>();
        foreach(WaterRepulsion water in waters) {
            water.AddListener(this);
        }
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
        position.y = collider.max.y + 0.2f;

        List<GameObject> effect = new List<GameObject>();
        if (size > 0) {
            GameObject anEffect = Instantiate(waves);
            anEffect.transform.position = position;
            ParticleSystem particle = anEffect.GetComponent<ParticleSystem>();
            particle.startSize *= size;
            particle.Stop();
            particle.Play();
            effect.Add(anEffect);
        }

        if (size >2) {
            GameObject anEffect = Instantiate(splash);
            anEffect.transform.position = position;
            ParticleSystem particle = anEffect.GetComponent<ParticleSystem>();
            particle.startSize *= size;
            ParticleSystem.ShapeModule shape = particle.shape;
            shape.radius = size * 0.5f;
            particle.Stop();
            particle.Play();
            effect.Add(anEffect);
        }

        if(size>4 && !exit){
            GameObject anEffect = Instantiate(columnWater);
            anEffect.transform.position = position;
            ParticleSystem particle = anEffect.GetComponent<ParticleSystem>();
            particle.startSize *= (size*0.3f);
            particle.Stop();
            particle.Play();
            effect.Add(anEffect);
        }

        //destroy system
        foreach (GameObject oneEffect in effect) {
            StartCoroutine(StopEmission(oneEffect));
            Destroy(oneEffect, time);
        }
    }

    private IEnumerator StopEmission(GameObject emissor) {
        ParticleSystem particle = emissor.GetComponent<ParticleSystem>();
        yield return new WaitForSeconds(particle.duration);
        ParticleSystem.EmissionModule module = particle.emission;
        ParticleSystem.MinMaxCurve rate = module.rate;
        rate.mode = ParticleSystemCurveMode.Constant;
        rate.constantMin = 0;
        rate.constantMax = 0;
        module.rate = rate;
    }
}
