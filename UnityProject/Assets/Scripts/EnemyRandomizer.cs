using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyRandomizer : MonoBehaviour
{

    public GameObject solarFlarePrefab;
    private GameObject solarFlare;

    public GameObject cometPrefab;
    private List<GameObject> comets;

    public float minWaitFlareTime = 5f;
    public float minWaitCometTime = 5f;

    private float cumulativeTime = 0.0f;

    // Use this for initialization
    void Start()
    {
        comets = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        cumulativeTime += Time.deltaTime;

        if (cumulativeTime >= minWaitFlareTime)
        {
            if (solarFlare != null)
            {
                GameObject.DestroyImmediate(solarFlare);
            }
            solarFlare = GameObject.Instantiate(solarFlarePrefab) as GameObject;

            float angle = Random.Range(0, 360);
            solarFlare.transform.Rotate(0, angle, 0);
            solarFlare.transform.localPosition = new Vector3(0, 10, 0);
            
            // Comet generation
            GameObject comet = GameObject.Instantiate(cometPrefab) as GameObject;
            comets.Add(comet);
            // TODO: Spawn outside the view frustum.
            // TODO: Target the current player position. 
            // TODO: Remove the comet when outside the window.

            cumulativeTime = 0;
        }
        
    }
}
