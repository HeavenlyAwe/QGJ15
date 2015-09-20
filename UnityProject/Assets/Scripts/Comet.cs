using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Comet : MonoBehaviour
{
    public Vector3 pos;
    public float targetY = 10.0f;
    public float yVelocity = 1.0f;

    void Update()
    {
        if (pos.y < targetY) {
            pos.y += yVelocity * Time.deltaTime;
        }
        transform.position = pos;
    }
}