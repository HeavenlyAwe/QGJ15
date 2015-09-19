using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SolarSystem : MonoBehaviour
{
    public float rotationSpeed = 10.0f;

    public GameObject sunPrefab;
    public GameObject atomCorePrefab;
    public GameObject orbitPrefab;
    public GameObject shipPrefab;

    public Material skyboxSpace;
    public Material skyboxAtom;

    public int state;

    public int numberOfOrbits = 5;

    public float hideHeight = -50;
    public List<Orbit> orbits;
    private GameObject sun;
    private GameObject atomCore;

    void Start()
    {
        sun = GameObject.Instantiate(sunPrefab);
        sun.transform.SetParent(transform);
        sun.transform.localPosition = Vector3.zero;

        atomCore = GameObject.Instantiate(atomCorePrefab);
        atomCore.transform.SetParent(transform);
        atomCore.transform.localPosition = Vector3.zero;

        // Creating ship object
        GameObject ship = GameObject.Instantiate(shipPrefab);
        ship.transform.SetParent(transform);
        ship.transform.localPosition = Vector3.zero;

        ship.GetComponent<PlayerController>().solarSystem = this;

        // Creating the orbits
        float offset = 0;
        for (int i = 1; i <= numberOfOrbits; i++)
        {
            Orbit orbit = CreateOrbit(3.0f + offset, i);
            offset += i + 0.5f;

            // The ship has references to all available orbits
            orbits.Add(orbit);
            ship.GetComponent<PlayerController>().SetOrbit(i - 1, i);
        }

        state = 1;
        SwapElements();
        state = 0;
    }

    public Orbit CreateOrbit(float radius, int planetCount)
    {
        Orbit orbit = GameObject.Instantiate(orbitPrefab).GetComponent<Orbit>();
        orbit.transform.SetParent(transform);
        orbit.transform.localPosition = Vector3.zero;

        orbit.radius = radius;
        orbit.CreateTrail();
        orbit.CreatePlanets(planetCount);
        orbit.CreateAtoms(planetCount);

        for (int i = 0; i < orbit.planets.Count; i++)
        {
            float tempRotationSpeed = rotationSpeed / Mathf.Pow(orbit.radius, 2);
            orbit.trailRotateSpeed = tempRotationSpeed;
            orbit.planets[i].rotationSpeed = tempRotationSpeed;
            // Set the speed of the underlying electron
            orbit.electrons[i].rotationSpeed = tempRotationSpeed;
        }

        return orbit;
    }


    public void SwitchState()
    {
        if (state == 0)
        {
            RenderSettings.skybox = skyboxAtom;
            float x = 8.0f;
            DOTween.To(() => x,
                       t => { x = t; RenderSettings.skybox.SetFloat("_Exposure", x); },
                       1.0f, 1.0f).SetEase(Ease.OutQuad);
            SwapElements();
            state = 1;
        }
        else
        {
            RenderSettings.skybox = skyboxSpace;
            float x = 8.0f;
            DOTween.To(() => x,
                       t => { x = t; RenderSettings.skybox.SetFloat("_Exposure", x); },
                       1.0f, 1.0f).SetEase(Ease.OutQuad);
            SwapElements();
            state = 0;
        }
    }

    /// <summary>
    /// If state == 0, we want to enable Atom mode, else Space mode
    /// </summary>
    private void SwapElements()
    {
        if (state == 0)
        {
            sun.SetActive(false);
            atomCore.SetActive(true);
        }
        else
        {
            sun.SetActive(true);
            atomCore.SetActive(false);
        }

        foreach (Orbit o in orbits)
        {
            for (int i = 0; i < o.planets.Count; i++)
            {
                if (state == 0)
                {
                    o.planets[i].height = hideHeight;
                    o.electrons[i].height = 0;
                }
                else
                {
                    o.planets[i].height = 0;
                    o.electrons[i].height = hideHeight;
                }
            }
        }

    }

}
