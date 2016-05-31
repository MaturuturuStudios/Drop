﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[ExecuteInEditMode]
public class WaterRepulsion : ShootVelocity {
    #region Public attributes
    public Transform pointExpulsion;
    /// <summary>
    /// Delay
    /// </summary>
    public float delay = 0.8f;

    public ParticleSystem particleEffectEnter;
	public ParticleSystem particleEffectExit;

    
    #endregion

    #region Private attributes
    /// <summary>
    /// Drops that touched the water
    /// </summary>
    private List<GameObject> _enteredDrop;

    private List<GameObject> _expelDrop;

    /// <summary>
    /// Bounds of the water
    /// </summary>
    private Bounds _ownCollider;
    #endregion

    #region Methods
    // Use this for initialization
    void Start () {
        //get the collider
        _ownCollider = GetComponent<Collider>().bounds;
        //create list
        _enteredDrop = new List<GameObject>();
        _expelDrop = new List<GameObject>();
    }

    public void Update() {
        //no drop? get out
        if (_enteredDrop.Count == 0) return;
        if (!Application.isPlaying) return;

        //for every drop in water...
        for (int i = _enteredDrop.Count; i > 0; i--) {
            GameObject drop = _enteredDrop[i-1];
            //get position and bounds
            Vector3 position = drop.transform.position;
            float halfSize = drop.GetComponent<CharacterSize>().GetSize() * 0.5f;
            //get four direction of drop
            Vector3[] vertices = new Vector3[4];
            vertices[0] = position;
            vertices[0].x -= halfSize;
            vertices[1] = position;
            vertices[1].x += halfSize;
            vertices[2] = position;
            vertices[2].y -= halfSize;
            vertices[3] = position;
            vertices[3].y += halfSize;


            //check if all points are inside the water
            bool result = true;
            for(int j=0; j<vertices.Length && result; j++) {
                result = _ownCollider.Contains(vertices[j]);
            }

            //is inside? get the drop out!
            if (result) {
                _enteredDrop.RemoveAt(i-1);
                //start the delay
                StartCoroutine(ExpelDrop(drop));
            }
        }
    }

    

    /// <summary>
    /// Check the drop inside the water
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other) {
        //get the component if is a drop
        GameObject drop = other.gameObject;
        if (drop.tag != Tags.Player) return;
        
        if (!_expelDrop.Contains(drop)) {
            _enteredDrop.Add(drop);

            //set particle effect (and inmediately destroy it)
			int scale=(int)(drop.transform.localScale.x);
			//position
			Vector3 position=drop.transform.position;
            ParticleEnter(position, scale);
			
        }
    }

    /// <summary>
    /// Remove the drops that is outside the water
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other) {
        //get the component if is a drop
        GameObject drop = other.gameObject;
        if (drop.tag != Tags.Player) return;
        _enteredDrop.Remove(drop);
        _expelDrop.Remove(drop);
    }

    private IEnumerator ExpelDrop(GameObject drop) {
        _expelDrop.Add(drop);
        drop.SetActive(false);
        yield return new WaitForSeconds(delay);
        drop.SetActive(true);
        CharacterControllerCustom controller = drop.GetComponent<CharacterControllerCustom>();
        //put drop on point expulsion
        drop.transform.position = pointShoot.position;

		//set particle effect
		int scale=(int)(drop.transform.localScale.x);
		//position
		Vector3 position=drop.transform.position;
        ParticleExit(position, scale);
		

        //send it flying (stop previous flying)
        controller.StopFlying();
        controller.SendFlying(GetNeededVelocityVector());
    }

	

	



    /// <summary>
    /// Modifier of the rate scale
    /// </summary>
    public float particleEmissionRateMultiplierScale = 0.5f;
    public float particleSizeMultiplierScale = 1f;
    public float particleSizeShapeMultiplierScale=1.0f;

    private void ParticleEnter(Vector3 position, float scale) {
        GameObject particleSystem = Instantiate(particleEffectEnter.gameObject) as GameObject;
        position.y = _ownCollider.max.y + 0.2f;
        particleSystem.GetComponent<Transform>().position = position;
        ParticleSystem[] system = particleSystem.GetComponentsInChildren<ParticleSystem>();

        ParticleSystem particle = system[0];

            //scale the emission burst
            ParticleSystem.EmissionModule emission = particle.emission;
            ParticleSystem.Burst[] burst=new ParticleSystem.Burst[emission.burstCount];
            emission.GetBursts(burst);
            burst[0].minCount *= (short)(particleEmissionRateMultiplierScale * scale);
            burst[0].maxCount *= (short)(particleEmissionRateMultiplierScale * scale);
            emission.SetBursts(burst);

            //scale the shape emission
            ParticleSystem.ShapeModule shape = particle.shape;
            shape.radius *= particleSizeShapeMultiplierScale* scale;


        particle = system[1];
        particle.startSize *= (0.5f * scale);
        ParticleSystem.VelocityOverLifetimeModule velocity = particle.velocityOverLifetime;
        ParticleSystem.MinMaxCurve x = velocity.y;
        x.constantMax *= scale;


        //destroy system
        Destroy(particleSystem, particleEffectEnter.startLifetime);
        
    }

    private void ParticleExit(Vector3 position, float scale) {
        ParticleEnter(position, scale);
    }

    protected override void OnAction(GameObject character) {
        //do nothing
    }

    public new void OnDrawGizmos() {
        if (!Application.isPlaying) {
            pointShoot = pointExpulsion;

            RaycastHit hitpoint;
            Vector3[] points = new Vector3[100];

            float finalAngle = angle;
            if (finalAngle < 0) {
                finalAngle = 180 + finalAngle;
            }
            float localAngle = finalAngle;
            

            float angleRadian = localAngle * Mathf.Deg2Rad;
            float velocity = base.GetNeededVelocity(angleRadian);

            float fTime = 0.1f;
            for (int i = 0; i < points.Length; i++) {
                float dx = velocity * fTime * Mathf.Cos(angleRadian);
                float dy = velocity * fTime * Mathf.Sin(angleRadian) - ((25) * fTime * fTime / 2.0f);

                Vector3 position = new Vector3(pointShoot.position.x + dx, pointShoot.position.y + dy, 0);
                points[i] = position;
                fTime += 0.1f;

                Gizmos.color = Color.green;
                if (i > 0) {
                    Vector3 f = points[i - 1] - points[i];
                    if ((Physics.Raycast(points[i], f, out hitpoint, f.magnitude, layerMask))) break;
                    else Gizmos.DrawRay(points[i], f);

                } else if (i == 0) {
                    Vector3 f = pointShoot.position - points[i];
                    Gizmos.DrawRay(points[i], f);
                }

            }
        }
    }
    #endregion
}
