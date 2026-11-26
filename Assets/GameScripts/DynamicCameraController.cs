using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject kart;
    public GameObject DynamicCamera;
    private Rigidbody kartrb;

    public AnimationCurve followCurve;

    private static float[] big_model_distances = { 36f, 72f, 8f, 25f};
    private static Vector3[] big_model_directions = { new Vector3(0, 20, -30).normalized, new Vector3(0, 20, -30).normalized, new Vector3(0, 80, -30).normalized, new Vector3(30, 80, -120).normalized };

    private static float[] small_model_distances = { 36f, 72f, 12f, 25f };
    private static Vector3[] small_model_directions = { new Vector3(0, 20, -30).normalized, new Vector3(0, 20, -30).normalized, new Vector3(0, 20, -30).normalized, new Vector3(30, 40, -120).normalized };

    private int currentCameraIndex;
    private bool isBigModel;

    private int carID = CarCarousel.ChosenCar;
    //private int carID = 3; // Uncomment this to debug a specific car (KartController, WheelRotation, DynamicCameraController should all be same)

    void Start()
    {
        currentCameraIndex = 0;
        isBigModel = (carID == 1 || carID == 2 );

        if(carID == 0)
        {
            // kart 1 - F1
            kart = GameObject.Find("player_kart_1");
        } else if(carID == 1)
        {
            // kart 2 - Monster Truck
            kart = GameObject.Find("player_kart_2");
        } else if(carID == 2)
        {
            // kart 3 - Truck
            kart = GameObject.Find("player_kart_3");
        } else
        {
            // kart 4 - Delorean
            kart = GameObject.Find("player_kart_4");
        }

        var kartPos = kart.transform.position;
        if( isBigModel )
        {
            DynamicCamera.transform.position = kartPos + (big_model_directions[currentCameraIndex] * big_model_distances[currentCameraIndex]);
        } else
        {
            DynamicCamera.transform.position = kartPos + (small_model_directions[currentCameraIndex] * small_model_distances[currentCameraIndex]);
        }
        
        transform.LookAt(kartPos);
        transform.rotation = transform.rotation * Quaternion.Euler(-10, 0, 0);

        kartrb = kart.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var kartPos = kart.transform.position;
        //var kartRot = kart.transform.rotation;

        var kartVelocity = kartrb.velocity;
        var maxVelocity = 45f;

        if (currentCameraIndex == 0 || currentCameraIndex == 1)
        {
            Quaternion targetRotation = Quaternion.Euler(kart.transform.eulerAngles.x, kart.transform.eulerAngles.y, 0);
            var result = followCurve.Evaluate(kartVelocity.magnitude / maxVelocity);
            var direction = Vector3.Dot(kart.transform.forward.normalized, kartVelocity.normalized);

            // Calculate the position relative to the target's position
            Vector3 desiredPosition;

            if (isBigModel)
            {
                desiredPosition = kartPos + targetRotation * ((big_model_directions[currentCameraIndex] * big_model_distances[currentCameraIndex]) + direction * (big_model_directions[currentCameraIndex] * 8 * result));
            }
            else
            {
                desiredPosition = kartPos + targetRotation * ((small_model_directions[currentCameraIndex] * small_model_distances[currentCameraIndex]) + direction * (small_model_directions[currentCameraIndex] * 8 * result));
            }

            transform.position = desiredPosition;
            transform.LookAt(kartPos);
            transform.rotation = transform.rotation * Quaternion.Euler(-10, 0, 0);
        }
        

        if (currentCameraIndex == 2 || currentCameraIndex == 3)
        {
            Quaternion targetRotation = Quaternion.Euler(kart.transform.eulerAngles.x, kart.transform.eulerAngles.y, 0);

            // Calculate the position relative to the target's position
            Vector3 desiredPosition;

            if (isBigModel)
            {
                desiredPosition = kartPos + targetRotation * big_model_directions[currentCameraIndex] * big_model_distances[currentCameraIndex];
            }
            else
            {
                desiredPosition = kartPos + targetRotation * small_model_directions[currentCameraIndex] * small_model_distances[currentCameraIndex];
            }

            transform.position = desiredPosition;
            Vector3 forwardAdjust;

            if (currentCameraIndex == 2 && isBigModel)
            {
                forwardAdjust = new Vector3(0, 7, 0);
            } else if (currentCameraIndex == 3 && isBigModel)
            {
                forwardAdjust = new Vector3(0, 5, 2);
            } else
            {
                forwardAdjust = new Vector3(0, 5, 0);
            }

            transform.LookAt(kart.transform.TransformPoint(kart.transform.forward) + forwardAdjust);
        }
    }
    void Update()
    {
        // Switch camera position based on user input (e.g., pressing the "C" key)
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentCameraIndex = (currentCameraIndex + 1) % big_model_directions.Length;
        }
    }
}
