using UnityEngine;
using System.Collections;

public class spin : MonoBehaviour {

    public float rotateX = 0;
    public float rotateY = 1.0f;
    public float rotateZ = 0;
    	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (rotateX, rotateY, rotateZ));
	}
}
