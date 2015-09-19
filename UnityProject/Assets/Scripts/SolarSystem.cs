﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SolarSystem : MonoBehaviour
{
    public float rotationSpeed = 10.0f;

    public GameObject orbitPrefab;
	public Skybox skybox;

    public int numberOfOrbits = 5;
    public int numberOfPlanets = 1;

    void Start()
    {
        for (int i = 0; i < numberOfOrbits; i++)
        {
            CreateOrbit(5.0f + i * 2.0f, numberOfPlanets);
        }
    }

    public void CreateOrbit(float radius, int planetCount)
    {
        var orbit = GameObject.Instantiate(orbitPrefab).GetComponent<Orbit>();
        orbit.transform.SetParent(transform);
        orbit.transform.localPosition = Vector3.zero;

        orbit.radius = radius;
        orbit.CreateTrail();
        orbit.CreatePlanets(planetCount);

        foreach (Planet p in orbit.planets)
        {
            p.rotationSpeed = rotationSpeed / Mathf.Pow(orbit.radius, 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space)) {
			//RenderSettings.skybox.SetFloat("_Exposure", 8.0f);
			float x = 8.0f;
			DOTween.To(() => x,
			           t => { x = t; RenderSettings.skybox.SetFloat("_Exposure", x); },
			           1.0f, 1.0f).SetEase(Ease.OutQuad);
		}
    }

}
