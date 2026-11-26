using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform middleRightWheel;
    public Transform middleLeftWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    public Transform frontRightSteer;
    public Transform frontLeftSteer;

    public float wheelRadius = .7f; // Radius in meters
    public float maxSteeringAngle = 30f; // Maximum steering angle in degrees

    private Rigidbody rb;
    private float steeringAngle;

    private int carID = CarCarousel.ChosenCar;
    //private int carID = 3; // Uncomment this to debug a specific car (KartController, WheelRotation, DynamicCameraController should all be same)

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        SteerWheels();
    }

    // FixedUpdate is called once per physics frame
    void FixedUpdate()
    {
        RotateWheels();
    }

    private void RotateWheels()
    {
        // Calculate the rotation angle based on the distance traveled
        float velocityMagnitude = rb.velocity.magnitude;
        float distanceTraveled = velocityMagnitude * Time.fixedDeltaTime;
        float rotationAngle = (distanceTraveled / (2 * Mathf.PI * wheelRadius)) * 360;

        // Rotate the wheels around the local Z axis for spinning
        if (carID == 3)
        {
            // Delorean requires wheels to be spun around local X axis
            frontLeftWheel.Rotate(Vector3.right, rotationAngle, Space.Self);
            frontRightWheel.Rotate(Vector3.right, rotationAngle, Space.Self);

            rearLeftWheel.Rotate(Vector3.right, rotationAngle, Space.Self);
            rearRightWheel.Rotate(Vector3.right, rotationAngle, Space.Self);
        } else {
            frontLeftWheel.Rotate(Vector3.forward, rotationAngle, Space.Self);
            frontRightWheel.Rotate(Vector3.forward, rotationAngle, Space.Self);

            rearLeftWheel.Rotate(Vector3.forward, rotationAngle, Space.Self);
            rearRightWheel.Rotate(Vector3.forward, rotationAngle, Space.Self);
        }

        if (carID == 2)
        {
            middleLeftWheel.Rotate(Vector3.forward, rotationAngle, Space.Self);
            middleRightWheel.Rotate(Vector3.forward, rotationAngle, Space.Self);
        }
    }

    private void SteerWheels()
    {
        // Get the steering input
        float horizontalInput = Input.GetAxis("Horizontal");

        // Calculate the steering angle based on the input
        steeringAngle = maxSteeringAngle * horizontalInput;

        // Apply the steering angle to the front wheels
        Quaternion steerRotation = Quaternion.Euler(0, steeringAngle, 0);


        frontLeftSteer.localRotation = steerRotation;
        frontRightSteer.localRotation = steerRotation;
    }
}
