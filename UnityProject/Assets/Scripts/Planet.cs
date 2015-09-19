using UnityEngine;
using System.Collections.Generic;

public class Planet : MonoBehaviour {
    
    public float rotationSpeed = 0;

    // Defines the depth of the image (how far away the planets are)
    public float height = 5;

    // azimuthal angle (moving around the trajectory)
    public float theta = 0f;

    public int direction;

    // Use this for initialization
    void Start()
    {
        SetDirectionRight();
    }


    public void SetDirectionRight()
    {
        direction = 1;
    }

    public void SetDirectionLeft()
    {
        direction = -1;
    }
    
}

