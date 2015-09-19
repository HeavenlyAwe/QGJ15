﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public ParticleSystem smoke;

    public Material skyboxSpace;
    public Material skyboxAtom;

    public float angle;
    public float radius;

    public float rotationImpulse = 0.01f;
    public float damping = 0.95f;
    public float acceleration = 0.1f;

    public int state;

    public float rotationSpeed;

    // link to the current orbit
    private List<Orbit> orbits = new List<Orbit>();
    private int currentOrbitIndex = -1;
    private float targetRadius;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Always apply the damping
        rotationSpeed *= damping;

        if (state == 0)
        {
            UpdateRotateInOrbital();
        }

        //==================================================================================
        // Movement between orbitals
        //==================================================================================

        if (state == 1)
        {
            UpdateSwitchOrbital();
        }

        //==================================================================================
        // Movement between Space and Atom state
        //==================================================================================
        UpdateSwapBetweenStates();

        if (orbits[currentOrbitIndex] != null)
        {
            targetRadius = orbits[currentOrbitIndex].radius;
        }
        radius = Mathf.Lerp(radius, targetRadius, 10.0f * Time.deltaTime);
        angle += rotationSpeed;

        transform.localPosition = new Vector3(radius * Mathf.Cos(angle), transform.localPosition.y, radius * Mathf.Sin(angle));
    }


    private void UpdateRotateInOrbital()
    {
        float orbitDamping = 4 / radius;
        // Add impulse to the rotationSpeed
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            smoke.Play();
            rotationSpeed = rotationImpulse * orbitDamping;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            smoke.Play();
            rotationSpeed = -rotationImpulse * orbitDamping;
        }

        // Continue the movement by applying a small acceleration
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotationSpeed += acceleration * orbitDamping * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rotationSpeed -= acceleration * orbitDamping * Time.deltaTime;
        }
    }

    private void UpdateSwitchOrbital()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentOrbitIndex += 1;
            if (currentOrbitIndex >= orbits.Count)
            {
                currentOrbitIndex = orbits.Count - 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentOrbitIndex -= 1;
            if (currentOrbitIndex < 0)
            {
                currentOrbitIndex = 0;
            }
        }
    }

    private void UpdateSwapBetweenStates()
    {
        // Functionality to swap between the Space and Atom worlds
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (state == 0)
            {
                RenderSettings.skybox = skyboxAtom;
                float x = 8.0f;
                DOTween.To(() => x,
                           t => { x = t; RenderSettings.skybox.SetFloat("_Exposure", x); },
                           1.0f, 1.0f).SetEase(Ease.OutQuad);
                state = 1;
            }
            else
            {
                RenderSettings.skybox = skyboxSpace;
                float x = 8.0f;
                DOTween.To(() => x,
                           t => { x = t; RenderSettings.skybox.SetFloat("_Exposure", x); },
                           1.0f, 1.0f).SetEase(Ease.OutQuad);
                state = 0;
            }
        }
    }


    public void AddOrbit(Orbit orbit)
    {
        orbits.Add(orbit);
        currentOrbitIndex += 1;
    }

}
