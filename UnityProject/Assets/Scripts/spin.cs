using UnityEngine;
using System.Collections;

public class spin : MonoBehaviour {

    public float rotateX = 0;
    public float rotateY = 1.0f;
    public float rotateZ = 0;

    public float rotationSpeed = 1;
    	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (rotateX * rotationSpeed * Time.deltaTime,
            rotateY * rotationSpeed * Time.deltaTime, rotateZ * rotationSpeed * Time.deltaTime));
	}
}
