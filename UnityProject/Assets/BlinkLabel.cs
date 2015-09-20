using UnityEngine;
using System.Collections;

public class BlinkLabel : MonoBehaviour {

    public float blinkPeriod = 2.0f;
    public float onTime = 1.5f;  // how long label is showing before hiding
    public float periodStart = 0.0f;
	
	// Update is called once per frame
	void Update () {
	    if (Time.time > periodStart + blinkPeriod)
        {
            periodStart = Time.time;
            GetComponent<MeshRenderer>().enabled = true;
        }
        if (Time.time > periodStart + onTime)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
	}
}
