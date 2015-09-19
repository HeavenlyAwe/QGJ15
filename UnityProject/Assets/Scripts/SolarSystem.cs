using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SolarSystem : MonoBehaviour
{
    public float rotationSpeed = 10.0f;

    public GameObject orbitPrefab;
    public GameObject shipPrefab;

    public Material skyboxSpace;
    public Material skyboxAtom;

    public int state;

    public int numberOfOrbits = 5;

    void Start()
    {
        // Creating ship object
        GameObject ship = GameObject.Instantiate(shipPrefab);
        ship.transform.SetParent(transform);
        ship.transform.localPosition = Vector3.zero;

        ship.GetComponent<PlayerController>().skyboxAtom = skyboxAtom;
        ship.GetComponent<PlayerController>().skyboxSpace = skyboxSpace;

        // Creating the orbits
        float offset = 0;
        for (int i = 1; i <= numberOfOrbits; i++)
        {
            Orbit orbit = CreateOrbit(3.0f + offset, i);
            offset += i + 0.5f;

            // The ship has references to all available orbits
            ship.GetComponent<PlayerController>().AddOrbit(orbit);
        }

    }

    public Orbit CreateOrbit(float radius, int planetCount)
    {
        Orbit orbit = GameObject.Instantiate(orbitPrefab).GetComponent<Orbit>();
        orbit.transform.SetParent(transform);
        orbit.transform.localPosition = Vector3.zero;

        orbit.radius = radius;
        orbit.CreateTrail();
        orbit.CreatePlanets(planetCount);

        foreach (Planet p in orbit.planets)
        {
            float tempRotationSpeed = rotationSpeed / Mathf.Pow(orbit.radius, 2);
            orbit.trailRotateSpeed = tempRotationSpeed;
            p.rotationSpeed = tempRotationSpeed;
        }

        return orbit;
    }

  //  // Update is called once per frame
  //  void Update()
  //  {
  //      // Functionality to swap between the Space and Atom worlds
		//if (Input.GetKeyDown(KeyCode.Space)) {
  //          if (state == 0)
  //          {
  //              RenderSettings.skybox = skyboxAtom;
  //              float x = 8.0f;
  //              DOTween.To(() => x,
  //                         t => { x = t; RenderSettings.skybox.SetFloat("_Exposure", x); },
  //                         1.0f, 1.0f).SetEase(Ease.OutQuad);
  //              state = 1;
  //          }
  //          else
  //          {
  //              RenderSettings.skybox = skyboxSpace;
  //              float x = 8.0f;
  //              DOTween.To(() => x,
  //                         t => { x = t; RenderSettings.skybox.SetFloat("_Exposure", x); },
  //                         1.0f, 1.0f).SetEase(Ease.OutQuad);
  //              state = 0;
  //          }
		//}
  //  }

}
