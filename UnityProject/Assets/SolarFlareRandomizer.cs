using UnityEngine;
using System.Collections;

public class SolarFlareRandomizer : MonoBehaviour {

    public GameObject solarFlarePrefab;
    private GameObject solarFlare;

    public float minWaitTime = 5f;
    private float cumulativeTime = 0.0f;

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        cumulativeTime += Time.deltaTime;

        if (cumulativeTime >= minWaitTime)
        {
            if(solarFlare != null)
            {
                GameObject.DestroyImmediate(solarFlare);
            }
            solarFlare = GameObject.Instantiate(solarFlarePrefab);
            
            float angle = Random.Range(0, 360);
            solarFlare.transform.Rotate(0, angle, 0);
            solarFlare.transform.localPosition = new Vector3(0, 10, 0);

            cumulativeTime = 0;
        }
	}
}
