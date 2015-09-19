using UnityEngine;
using System.Collections.Generic;

public class Orbit : MonoBehaviour
{

    public List<GameObject> planetPrefabs;
    public GameObject trailPrefab;

    public List<Planet> planets;
    
    public float radius = 1.0f;

    /*private float _radius = 1.0f;
    public float radius
    {
        get { return _radius; }
        set
        {
            _radius = value;
            CreateTrail();
        }
    }*/
    public int maxPlanets = 4;
    public float trailSpacing = 0.1f;

    public float trailPulsePeriod = 1.0f;
    public float trailPulseAmplitude = 0.1f;
    public float trailPulsePow = 2.0f;
    public float trailRotateSpeed = 1.0f;
    public float trailPulseDisplacement = 0.0f;

    void Start()
    {
        CreateTrail();
        trailPulseDisplacement = Random.Range(0, 3);
        //CreatePlanets(3);
    }

    public void CreatePlanets(int n)
    {
        for (int i = 0; i < n; i++)
        {
            int planetTypeIndex = Random.Range(0, planetPrefabs.Count);
            var prefab = planetPrefabs[planetTypeIndex];
            var p = GameObject.Instantiate(prefab).GetComponent<Planet>();
            p.transform.SetParent(transform);
            
            p.gameObject.name = "Planet" + i;
            p.theta = 2.0f * Mathf.PI / n;
            planets.Add(p);
        }
    }

    // Visual trail that shows where the orbiting planets can move
    private GameObject trail = null;
    public void CreateTrail()
    {
        // refresh by replacing old trail
        if (trail != null)
        {
            DestroyImmediate(trail);
        }
        trail = new GameObject();
        trail.name = "Trail";
        trail.transform.parent = transform;
        trail.transform.localPosition = Vector3.zero;

        int trailCount = Mathf.FloorToInt(Mathf.PI * 2.0f * radius / trailSpacing);
        float deltaAngle = Mathf.PI * 2.0f / trailCount;
        for (float a = 0.0f; a < Mathf.PI * 2.0f; a += deltaAngle)
        {
            var b = a + Mathf.PI / 2.0f;
            var t = GameObject.Instantiate(trailPrefab);
            t.name = "TrailMarker";
            t.transform.parent = trail.transform;
            t.transform.localPosition = new Vector3(
                Mathf.Cos(a) * radius, 0, Mathf.Sin(a) * radius);
            t.transform.localRotation = Quaternion.LookRotation(new Vector3(
                Mathf.Cos(b) * radius, 0.0f, Mathf.Sin(b) * radius));
        }
    }

    void Update()
    {
        // Rotate planets
        foreach (Planet p in planets)
        {
            float positionX = radius * Mathf.Cos(p.theta); // * Mathf.Sin(phi * Mathf.Deg2Rad);
            float positionZ = radius * Mathf.Sin(p.theta); // * Mathf.Sin(phi * Mathf.Deg2Rad);
            p.transform.localPosition = new Vector3(positionX, 0, positionZ);

            p.theta = p.theta + p.direction * p.rotationSpeed * Time.deltaTime;
        }

        // Trail effects: rotate/pulse
        if (trail != null)
        {
            trail.transform.Rotate(0.0f, trailRotateSpeed, 0.0f);

            // Pulsate
            float t = (Time.time + trailPulseDisplacement) % trailPulsePeriod;
            float s = 1.0f + trailPulseAmplitude * Mathf.Pow(Mathf.Abs(t - 0.5f * trailPulsePeriod), trailPulsePow);
            trail.transform.localScale = new Vector3(s, s, s);
        }
    }
}
