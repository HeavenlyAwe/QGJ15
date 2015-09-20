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
    public GameObject mainMenuUI;

    public GameObject explosion;

    public Material skyboxSpace;
    public Material skyboxAtom;

    public int numberOfOrbits = 5;

    public float hideHeight = -50;
    public List<Orbit> orbits;
    private GameObject sun;
    private GameObject atomCore;

    public GameObject solarFlarePrefab;
    private GameObject solarFlare;

    public GameObject cometPrefab;

    public float minWaitFlareTime = 5f;
    public float cometSpawnPeriod = 5f;

    private float cumulativeFlareTime = 0.0f;
    private float cumulativeCometTime = 0.0f;

    public List<Comet> comets;

    public AudioSource atomSpaceSwitchAudioSource;
    public AudioSource startGameAudioSource;

    public AudioSource mainMenuBackgroundMusic;
    public AudioSource gameBackgroundMusic;

    public Vector3 shipMainMenuPosition;

    public PlayerController ship;
    public enum State
    {
        MainMenu,
        Space,
        Atom
    }
    public State state;

    void Start()
    {
        sun = GameObject.Instantiate(sunPrefab);
        sun.transform.SetParent(transform);
        sun.transform.localPosition = Vector3.zero;

        atomCore = GameObject.Instantiate(atomCorePrefab);
        atomCore.transform.SetParent(transform);
        atomCore.transform.localPosition = Vector3.zero;

        shipMainMenuPosition = ship.transform.localPosition;
        explosion.SetActive(false);

        SetState(State.MainMenu);
    }

    // after player has pressed start in main menu
    void StartGame()
    {
        if (state != State.MainMenu) return;

        explosion.SetActive(false);

        mainMenuBackgroundMusic.Stop();
        startGameAudioSource.Play();
        gameBackgroundMusic.Play();
        mainMenuUI.SetActive(false);

        // Destroy old orbits
        foreach (Orbit o in orbits)
        {
            GameObject.DestroyImmediate(o.gameObject);
        }
        orbits = new List<Orbit>();

        // Creating new orbits
        float offset = 0;
        for (int i = 1; i <= numberOfOrbits; i++)
        {
            Orbit orbit = CreateOrbit(3.0f + offset, i);
            offset += i + 0.5f;

            // The ship has references to all available orbits
            orbits.Add(orbit);
            ship.SetOrbit(i - 1, i);
        }

        foreach (Comet c in comets)
        {
            GameObject.DestroyImmediate(c.gameObject);
        }
        comets = new List<Comet>();

        DOTween.To(t => ship.entryT = t, 0.0f, 1.0f, 1.0f);

        SetState(State.Space);
    }

    public void EndGame()
    {
        if (state == State.MainMenu) return;

        explosion.transform.localPosition = ship.transform.localPosition;
        explosion.SetActive(true);

        gameBackgroundMusic.DOFade(0.0f, 2.0f).OnComplete(() => gameBackgroundMusic.Stop());

        mainMenuBackgroundMusic.Play();
        mainMenuUI.SetActive(true);

        SetState(State.MainMenu);
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

    // called from PlayerController when Key.SPACE is pressed
    public void SwitchState()
    {
        switch (state) {
            case State.MainMenu:
                StartGame();
                break;
            case State.Space:
                SetState(State.Atom);
                break;
            case State.Atom:
                SetState(State.Space);
                break;
        }
    }

    // set internal state and update visuals
    private void SetState(State newState)
    {
        // TODO: check state transition validity...
        
        sun.SetActive(newState == State.Space);
        atomCore.SetActive(newState == State.Atom);

        switch (newState)
        {
            case State.MainMenu:
                { 
                    gameBackgroundMusic.Stop();
                    mainMenuBackgroundMusic.Play();
                    ship.transform.localPosition = shipMainMenuPosition;
                }
                break;
            case State.Space:
                {
                    atomSpaceSwitchAudioSource.Play();
                    RenderSettings.skybox = skyboxSpace;
                    // don't do flash effect when game starts
                    if (state != State.MainMenu)
                    {
                        float x = 8.0f;
                        DOTween.To(() => x,
                                   t => { x = t; RenderSettings.skybox.SetFloat("_Exposure", x); },
                                   1.0f, 1.0f).SetEase(Ease.OutQuad);
                    }
                }
                break;
            case State.Atom:
                {
                    atomSpaceSwitchAudioSource.Play();
                    RenderSettings.skybox = skyboxAtom;
                    // don't do flash effect when game starts
                    if (state != State.MainMenu)
                    {
                        float x = 8.0f;
                        DOTween.To(() => x,
                                   t => { x = t; RenderSettings.skybox.SetFloat("_Exposure", x); },
                                   1.0f, 1.0f).SetEase(Ease.OutQuad);
                    }
                }
                break;
        }

        if (newState != State.MainMenu)
        {
            foreach (Orbit o in orbits)
            {
                foreach (Planet p in o.planets)
                {
                    p.height = (newState == State.Space) ? 0 : hideHeight;
                }
                foreach (Planet p in o.electrons)
                {
                    p.height = (newState == State.Space) ? hideHeight : 0;
                }
            }
        }

        state = newState;
    }

    // Update is called once per frame
    void Update()
    {
        cumulativeFlareTime += Time.deltaTime;

        if (cumulativeFlareTime >= minWaitFlareTime)
        {
            if (solarFlare != null)
            {
                GameObject.DestroyImmediate(solarFlare);
            }
            solarFlare = GameObject.Instantiate(solarFlarePrefab) as GameObject;

            float angle = Random.Range(0, 360);
            solarFlare.transform.Rotate(0, angle, 0);
            solarFlare.transform.localPosition = new Vector3(0, 10, 0);

            cumulativeFlareTime = 0;
        }

        if (cumulativeCometTime > cometSpawnPeriod)
        {
            // Comet generation
            var comet = GameObject.Instantiate(cometPrefab).GetComponent<Comet>();

            // TODO: Spawn outside the view frustum.
            float a = Random.Range(0, 2 * Mathf.PI);
            comet.pos = new Vector3(Mathf.Cos(a) * 20, 0, Mathf.Sin(a) * 20);
            // TODO: Target the current player position. 
            // TODO: Remove the comet when outside the window.
            comet.transform.localPosition = new Vector3();
        }
    }

}
