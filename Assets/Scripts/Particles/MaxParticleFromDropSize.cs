using UnityEngine;
using System.Collections;

public class MaxParticleFromDropSize : MonoBehaviour {

    public ParticleSystem bubles;
    private GameControllerIndependentControl _gcic;

    void Awake() {
        _gcic = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerIndependentControl>();
    }

    // Use this for initialization
    void Start () {
        /*ParticleSystem.EmissionModule em = bubles.emission;
        float rate = _gcic.currentCharacter.GetComponent<CharacterSize>().GetSize();
        em.rate = new ParticleSystem.MinMaxCurve(rate);*/

        bubles.maxParticles = _gcic.currentCharacter.GetComponent<CharacterSize>().GetSize(); ;

    }
}
