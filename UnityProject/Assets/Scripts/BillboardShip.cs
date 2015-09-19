using UnityEngine;
using System.Collections;

public class BillboardShip : MonoBehaviour {

    public Transform targetTransform;
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(targetTransform.position, Vector3.up);
    }
}
