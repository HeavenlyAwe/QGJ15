using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    // link to the current orbit
    public Orbit orbit;

    public float angle;
    public float radius;

    public float rotationImpulse = 0.01f;
    public float damping = 0.95f;
    public float acceleration = 0.1f;

    private float rotationSpeed;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Always apply the damping
        rotationSpeed *= damping;

        // Add impulse to the rotationSpeed
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rotationSpeed = rotationImpulse;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rotationSpeed = -rotationImpulse;
        }

        // Continue the movement by applying a small acceleration
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotationSpeed += acceleration * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rotationSpeed -= acceleration * Time.deltaTime;
        }
        angle += rotationSpeed;

        if (orbit != null)
        {
            radius = orbit.radius;
        }

        transform.localPosition = new Vector3(radius * Mathf.Cos(angle), transform.localPosition.y, radius * Mathf.Sin(angle));
    }


}
