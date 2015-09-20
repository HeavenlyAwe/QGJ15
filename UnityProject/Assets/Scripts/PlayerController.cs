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

    public List<AudioClip> movementSounds;
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
        //==================================================================================
        // Movement between Space and Atom state
        //==================================================================================
        if (Input.GetKeyDown(KeyCode.Space))
        {
            followPlanet = false;
            planet = null;
            solarSystem.SwitchState();
        }

        if (solarSystem.state == SolarSystem.State.MainMenu) return;
            
        // Always apply the damping
        rotationSpeed *= damping;

        if (solarSystem.state == SolarSystem.State.Space)
        {
            UpdateRotateInOrbital();
        }

        //==================================================================================
        // Movement between orbitals
        //==================================================================================

        if (solarSystem.state == SolarSystem.State.Atom)
        {
            UpdateSwitchOrbital();
        }

        //if (solarSystem.orbits == null) return;
        if (solarSystem.orbits.Count <= currentOrbitIndex || currentOrbitIndex < 0)
        {
            currentOrbitIndex = solarSystem.orbits.Count - 1;
        }
        if (solarSystem.orbits[currentOrbitIndex] != null)
        {
            targetRadius = solarSystem.orbits[currentOrbitIndex].radius;
        }
        radius = Mathf.Lerp(radius, targetRadius, 10.0f * Time.deltaTime);
        angle += rotationSpeed;

        var target = new Vector3(
            radius * Mathf.Cos(angle), 
            0.0f, //transform.localPosition.y, 
            radius * Mathf.Sin(angle));
        transform.localPosition = Vector3.Lerp(solarSystem.shipMainMenuPosition, target, entryT);
    }
    public float entryT = 0.0f;

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
            int i = Random.Range(0, movementSounds.Count);
            audioSource.PlayOneShot(movementSounds[i], 1.0f);
            rotationSpeed = rotationImpulse * orbitDamping;
            followPlanet = false;
            planet = null;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            smoke.Play();
            int i = Random.Range(0, movementSounds.Count);
            audioSource.PlayOneShot(movementSounds[i], 1.0f);
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



    /// <summary>
    /// Make the player start on the latest added orbit
    /// </summary>
    /// <param name="orbit"></param>
    public void SetOrbit(int index, int numberOfPlanetsInOrbit)
    {
        currentOrbitIndex = index;

        startOrbitIndex = currentOrbitIndex;
        startAngle = -0.5f * Mathf.PI;// / (numberOfPlanetsInOrbit * 4);

        rotationSpeed = 0.0f;
        angle = startAngle;
        radius = solarSystem.orbits[currentOrbitIndex].radius;
        targetRadius = radius;
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
            solarSystem.EndGame();
        }
    }

}
