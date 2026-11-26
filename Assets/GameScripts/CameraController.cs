using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Transform[] cameraPositions; // Array of camera positions
    public float smoothSpeed = 0.125f;

    private int currentCameraIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Set the initial camera position based on the current index
        transform.position = cameraPositions[currentCameraIndex].position;
        transform.rotation = cameraPositions[currentCameraIndex].rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Get the desired position and rotation from the current camera position
        Vector3 desiredPosition = cameraPositions[currentCameraIndex].position;
        Quaternion desiredRotation = cameraPositions[currentCameraIndex].rotation;

        // Smoothly move the camera to the desired position and rotation
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed);

        transform.position = smoothedPosition;
        transform.rotation = smoothedRotation;
    }

    void Update()
    {
        // Switch camera position based on user input (e.g., pressing the "C" key)
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentCameraIndex = (currentCameraIndex + 1) % cameraPositions.Length;
        }
    }
}
