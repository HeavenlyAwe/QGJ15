using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public ParticleSystem smoke;

    // Reference to the SolarSystem object for controlling the swapping
    public SolarSystem solarSystem;

    public float angle;
    public float radius;

    public float rotationImpulse = 0.01f;
    public float damping = 0.95f;
    public float acceleration = 0.1f;

    public AudioClip explosionSound;
    private AudioSource audioSource;

    private float rotationSpeed;

    // link to the current orbit
    private int currentOrbitIndex = -1;
    private float targetRadius;

    private float startAngle = 0.0f;
    private int startOrbitIndex = -1;

    private bool followPlanet = false;
    private Planet planet;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Always apply the damping
        rotationSpeed *= damping;

        if (solarSystem.state == 0)
        {
            UpdateRotateInOrbital();
        }

        //==================================================================================
        // Movement between orbitals
        //==================================================================================

        if (solarSystem.state == 1)
        {
            UpdateSwitchOrbital();
        }

        //==================================================================================
        // Movement between Space and Atom state
        //==================================================================================
        UpdateSwapBetweenStates();

        if (solarSystem.orbits[currentOrbitIndex] != null)
        {
            targetRadius = solarSystem.orbits[currentOrbitIndex].radius;
        }
        radius = Mathf.Lerp(radius, targetRadius, 10.0f * Time.deltaTime);
        angle += rotationSpeed;

        transform.localPosition = new Vector3(radius * Mathf.Cos(angle), transform.localPosition.y, radius * Mathf.Sin(angle));
    }


    private void UpdateRotateInOrbital()
    {
        if (followPlanet)
        {
            angle = planet.theta;
        }

        float orbitDamping = 4 / radius;
        // Add impulse to the rotationSpeed
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            smoke.Play();
            rotationSpeed = rotationImpulse * orbitDamping;
            followPlanet = false;
            planet = null;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            smoke.Play();
            rotationSpeed = -rotationImpulse * orbitDamping;
            followPlanet = false;
            planet = null;
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
            followPlanet = false;
            planet = null;
            currentOrbitIndex += 1;
            if (currentOrbitIndex >= solarSystem.orbits.Count)
            {
                currentOrbitIndex = solarSystem.orbits.Count - 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            followPlanet = false;
            planet = null;
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
            followPlanet = false;
            planet = null;
            solarSystem.SwitchState();
        }
    }

    /// <summary>
    /// Make the player start on the latest added orbit
    /// </summary>
    /// <param name="orbit"></param>
    public void SetOrbit(int index, int numberOfPlanetsInOrbit)
    {
        currentOrbitIndex = index;

        startOrbitIndex = currentOrbitIndex;
        startAngle = 2.0f * Mathf.PI / (numberOfPlanetsInOrbit * 2);

        ResetPlayer();
    }

    /// <summary>
    /// The player handles the collision detection. Checks whether 
    /// the collider it collides with belongs to some "Enemy".
    /// 
    /// The ShipPrefab object has a Rigidbody for this system to work,
    /// however it is set to "kinematic" to skip all the physic stuff.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Planet")
        {
            followPlanet = true;
            planet = other.GetComponentInParent<Planet>();
        }
        else
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(explosionSound, 1.0f);
            }
            ResetPlayer();
        }
    }
    
    public void ResetPlayer()
    {
        currentOrbitIndex = startOrbitIndex;
        rotationSpeed = 0.0f;
        angle = startAngle;
        radius = solarSystem.orbits[currentOrbitIndex].radius;
        targetRadius = radius;
    }

}
