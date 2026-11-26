using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;


public class KartController : MonoBehaviour
{
    public GameObject kartGameObject;
    // Kart handling
    public float suspensionHeight;
    public float springStrength;
    public float springDamping;

    // Tire grip is value [0, 1] where 0 is no grip
    // and 1 is maximum grip
    public float frontTireGrip;
    public float backTireGrip;
    public float tireMass; // Should be roughly 2% of kart mass

    // Kart acceleration, turn speed and top speed
    public float accelSpeed;
    public float turnSpeed;
    public float topSpeed;

    // Lets us to calculate the torque available in the engine
    // as a function of the cars current speed
    public AnimationCurve torqueCurve;

    // Lap information
    public int currentCheckpoint;
    public int currentLap;
    public TextMeshProUGUI lapCounter;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI split;
    public TextMeshProUGUI speedometer;
    private float timeInCurrentLap = 0;
    private float lastLapTime = -1;

    // Raycasting variables
    public Transform startPosition;
    public Transform kart;
    public Transform[] tirePositions;

    public PlayableDirector raceCountdownTrigger;

    private Rigidbody rb;

    private int selectedCar = CarCarousel.ChosenCar;
    //private int selectedCar = 3; // Uncomment this to debug a specific car (KartController, WheelRotation, DynamicCameraController should all be same)
    public static Boolean isWaiting = true;
    
    public string loseSceneName = "LoseScene";
    // private int selectedCar = 3; // Uncomment this to debug a specific car (KartController, WheelRotation, DynamicCameraController should all be same)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InitializeTorqueCurve();
        lapCounter.text = "Lap: " + "1" + "/3";

        String carId = (selectedCar + 1) + "";
        Debug.Log("i " + carId + ", " + this.transform.name + " should be active!");

        bool active = true;
        if (!this.transform.name.Contains(carId))
        {
            Debug.Log("deactivating " + carId + ", " + this.transform.name);
            kartGameObject.SetActive(false);
            active = false;
        } else
        {
            // place in positions
            kartGameObject.transform.SetLocalPositionAndRotation(new Vector3(0, 1, 0), transform.rotation);   
        }

        if(active)
        {
            isWaiting = true;
            StartCoroutine(Wait()); // start the coroutine to wait
            raceCountdownTrigger.Play();

        }

    }

    IEnumerator Wait()
    {
        isWaiting = true;  //set the bool to stop moving
        print("Start to wait");
        yield return new WaitForSeconds(3); // wait for 5 sec
        print("Wait complete");
        isWaiting = false; // set the bool to start moving

    }


    void InitializeTorqueCurve()
    {
        torqueCurve = new AnimationCurve();

        // Define key points of the torque curve
        torqueCurve.AddKey(0f, 0.5f); // At 0% speed, 50% torque
        torqueCurve.AddKey(0.1f, 0.6f); // At 10% speed, 60% torque
        torqueCurve.AddKey(0.2f, 0.8f); // At 20% speed, 80% torque
        torqueCurve.AddKey(0.3f, 0.9f); // At 30% speed, 90% torque
        torqueCurve.AddKey(0.4f, 1f); // At 40% speed, 100% torque
        torqueCurve.AddKey(0.5f, 1f); // At 50% speed, 100% torque
        torqueCurve.AddKey(0.6f, 1f); // At 60% speed, 100% torque
        torqueCurve.AddKey(0.7f, 1f); // At 70% speed, 100% torque
        torqueCurve.AddKey(0.8f, 0.6f); // At 80% speed, 60% torque
        torqueCurve.AddKey(0.9f, 0.4f); // At 90% speed, 40% torque
        torqueCurve.AddKey(1f, 0.25f); // At 100% speed, 25% torque
    }

    void FixedUpdate()
    {
        if(!isWaiting)
        {
            HandleForces();
            CheckOutOfBounds();
            UpdateTimers();
            UpdateSpeedometer();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.position = startPosition.position;
            rb.rotation = startPosition.rotation;
            rb.velocity = Vector3.zero;
        }
    }

    void UpdateTimers()
    {
        timeInCurrentLap += Time.deltaTime;

        UpdateMainTime();
        UpdateSplit();

    }

    void UpdateSpeedometer() {
        var direction = Vector3.Dot(kart.transform.forward.normalized, rb.velocity.normalized);
        float mpsToMph = 2.23f;
        speedometer.text = "" + Mathf.Sign(direction) * Mathf.FloorToInt(rb.velocity.magnitude * mpsToMph);
    }

    void UpdateMainTime()
    {
        String extraZero = "";

        var minutes = Mathf.FloorToInt(timeInCurrentLap / 60f);
        var seconds = Mathf.FloorToInt(timeInCurrentLap % 60);

        if (seconds < 10)
        {
            extraZero = "0";
        }

        timer.text = minutes + ":" + extraZero + seconds;
    }

    void UpdateSplit()
    {
        if (lastLapTime > 0)
        {
            var deltaTime = Mathf.FloorToInt(timeInCurrentLap - lastLapTime);

            String extraZero = "";
            String modifier;
            Color color;

            if (deltaTime < 0)
            {
                modifier = "-";
                color = Color.green;
            }
            else if (deltaTime == 0)
            {
                modifier = " ";
                color = Color.yellow;
            }
            else
            {
                modifier = "+";
                color = Color.red;
            }

            deltaTime = Mathf.Abs(deltaTime);

            var minutes = Mathf.FloorToInt(deltaTime / 60f);
            var seconds = Mathf.FloorToInt(deltaTime % 60);

            if (seconds < 10)
            {
                extraZero = "0";
            }

            // get rid of weird negative stuff

            split.text = modifier + minutes + ":" + extraZero + seconds;
            split.color = color;


        }
    }

    public float GetCurrentSpeed()
    {
        return rb.velocity.magnitude;
    }

    void HandleForces()
    {
        float steerInput = Input.GetAxis("Horizontal");
        float accelInput = Input.GetAxis("Vertical");

        // Apply forward and backward force
        Vector3 movement = transform.forward * accelInput * accelSpeed;

        // Apply torque for turning
        float turn = steerInput * turnSpeed;

        int index = 0;
        bool isBackTire = true;
        

        foreach (Transform tire in tirePositions)
        {
            bool isBackLeftTire; // Necessary for semi
            if (index >= 2)
            {
                isBackTire = false;
            }

            if (index == 1)
            {
                isBackLeftTire = true;
            }
            else isBackLeftTire = false;


            RaycastHit hit;
            bool rayContact = Physics.Raycast(tire.position, -transform.up, out hit, suspensionHeight);

            if (rayContact)
            {
                rb.AddForce(movement);
                rb.AddTorque(transform.up * turn);

                SuspensionForce(tire, hit, isBackTire, isBackLeftTire);
                SteeringForce(tire, isBackTire, isBackLeftTire);
                AccelerationForce(tire, accelInput, isBackTire);
            } 
            else
            {
                rb.AddForce(movement / 10);
                rb.AddTorque(transform.up * (turn / 2));

                if (selectedCar == 1)
                {
                    // Give the monster truck some extra bounce
                    rb.AddForce(Vector3.down * rb.mass/6 * 9.8f);
                } else
                {
                    // Add gravitational force when the kart is in air
                    rb.AddForce(Vector3.down * rb.mass * 9.8f);
                }
                
            }

            index++;
        }
    }

    // Spring suspension physics
    void SuspensionForce(Transform tire, RaycastHit hit, bool isBackTire, bool isBackLeftTire)
    {
        Vector3 springForceDir;

        if(selectedCar == 2 && isBackLeftTire)
        {
            springForceDir = -tire.right;
        } else if (selectedCar == 2 && isBackTire)
        {
            springForceDir = tire.right;
        } 
        else
        {
            // Get the direction of the spring force
            springForceDir = tire.up;
        }
        

        // Get the velocity of the tire
        Vector3 tireVelocity = rb.GetPointVelocity(tire.position);

        // Calculate offset between suspension height and hit distance
        float offset = suspensionHeight - hit.distance;


        // calculate velocity along the direction of the spring
        float springDirectionVelocity = Vector3.Dot(springForceDir, tireVelocity);

        // Calculate spring force w/ dampening
        float springForce = (offset * springStrength) - (springDirectionVelocity * springDamping);

        // Apply the spring force along the direction of the spring at the location of the tire
        rb.AddForceAtPosition(springForceDir * springForce, tire.position);

        // Draw the ray for debugging
        //Debug.DrawRay(tire.position, springForceDir * springForce, Color.red);
    }

    // Steering force physics
    void SteeringForce(Transform tire, bool isBackTire, bool isBackLeftTire)
    {
        float tireGrip;

        // Differentiating between the grip values for the front and back tires
        // allows us more granularity in physics simulation and sick drift mechanics
        if (isBackTire)
        {
            tireGrip = backTireGrip;
        }
        else
        {
            tireGrip = frontTireGrip;
        }

        Vector3 steerForceDir;

        if (selectedCar == 2 && isBackLeftTire)
        {
            steerForceDir = -tire.forward;
        } else if (selectedCar == 2 && isBackTire)
        {
            steerForceDir = tire.forward;
        }
        else
        {
            // Grab the direction of the steering force 
            steerForceDir = tire.right;
        }

        // Get the velocity of the tire
        Vector3 tireVelocity = rb.GetPointVelocity(tire.position);

        // Get the tire's velocity in the direction of the steering
        float steeringVelocity = Vector3.Dot(steerForceDir, tireVelocity);

        // Calculate the change in velocity resulting from force acting opposite of the steering velocity
        float velocityChange = -steeringVelocity * tireGrip;

        // Turn the change in velocity into an acceleration
        float accelChange = velocityChange / Time.fixedDeltaTime;

        // Force = Mass * accelaration so multiply by tire mass and add as a force
        rb.AddForceAtPosition(steerForceDir * tireMass * accelChange, tire.position);

        // Draw the ray for debugging
        //Debug.DrawRay(tire.position, steerForceDir * tireMass * accelChange, Color.blue);
    }

    // Acceleration force physics
    void AccelerationForce(Transform tire, float accelInput, bool isBackTire)
    {
        Vector3 accelDir;

        if (selectedCar == 2 && isBackTire)
        {
            accelDir = tire.up;
        }
        else
        {
            // Get direction of acceleration force
            accelDir = tire.forward;
        }

        // Calculate acceleration torque
        if (accelInput > 0.0f)
        {
            // Forward speed of the kart
            float kartSpeed = Vector3.Dot(kart.forward, rb.velocity);
            //Debug.Log("The kart speed is: " + kartSpeed);

            // Find the percentage of top speed the kart is currentlt moving at
            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(kartSpeed) / topSpeed);

            //Debug.Log("The normal speed is: " + normalizedSpeed);


            // Access the torque curve to calculate how much torque to apply to the acceleration
            float availableTorque = torqueCurve.Evaluate(normalizedSpeed) * accelInput;

            //Debug.Log("The available torque is: " + availableTorque);

            // Add the force from torque in the direction of acceleration at the position of the tires
            rb.AddForceAtPosition(accelDir * availableTorque, tire.position);

            // Draw the ray for debugging
            //Debug.DrawRay(tire.position, accelDir * availableTorque, Color.green);
        }
    }

    void CheckOutOfBounds()
    {
        if (rb.transform.position.y < -50 || rb.transform.position.y > 100)
        {
            Debug.Log("Out of bounds! (y < -50 or y > 100)");
            ResetToLastCheckpoint();
        }
    }

    void ResetToLastCheckpoint()
    {
        //Debug.Log("Spawning at last checkpoint!");
        var allCheckpoints = Resources.FindObjectsOfTypeAll<CheckpointComponent>();

        for (int i = 0; i < allCheckpoints.Length; i++)
        {
            if (allCheckpoints[i].checkpointNumber == currentCheckpoint)
            {

                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.rotation = Quaternion.identity;
                rb.rotation = allCheckpoints[i].transform.rotation;

                // yes this is cursed
                Vector3 checkpointPosition = allCheckpoints[i].transform.position;
                rb.transform.position = checkpointPosition;
                rb.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y+0, rb.transform.position.z);

                Debug.Log("Kart pos: " + rb.transform.position);
                Debug.Log("checkpoint pos: " + checkpointPosition);

                RaycastHit[] hits = Physics.RaycastAll(kart.position, -transform.up, 500f);

                float closestObj = -1f;
                Vector3 hitpt = Vector3.zero;
                // Iterate through the hits
                foreach (RaycastHit hit in hits)
                {
                    //Debug.Log("Raycast hit: " + hit.collider.gameObject.name + " dist:" + hit.distance);
                    if(hit.collider.transform.name.Contains("Agent"))
                    {
                        //Debug.Log("hit a agent");
                        continue;
                    }
                    if (hit.collider.gameObject.GetComponent<KartController>() != null)
                    {
                        continue;
                    }
                    if (hit.collider.gameObject.GetComponent<CheckpointComponent>() != null)
                    {
                        continue;
                    }

                    if (hit.point.y > closestObj || closestObj == -1f)
                    {
                        closestObj = hit.point.y;
                        hitpt = hit.point;

                    }
                }
                if (closestObj == -1f)
                {
                    closestObj = 0f;
                }

                //Debug.Log(closestObj);

                var curPos = rb.transform.position;
                curPos.y = hitpt.y; //curPos.y -= (closestObj);
                curPos.y += 1;
                Debug.Log("before pos: " + rb.transform.position);
                rb.transform.position = curPos;
                Debug.Log("after pos: " + rb.transform.position);


            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckpointComponent checkpoint = other.GetComponent<CheckpointComponent>();
        if (checkpoint != null)
        {
            var checkpointNumberJustHit = checkpoint.checkpointNumber;

            if (checkpointNumberJustHit == currentCheckpoint + 1)
            {
                currentCheckpoint = checkpointNumberJustHit;
            }
            //Debug.Log("Current checkpoint is " + currentCheckpoint);
            return;
        }

        FinishLineComponent finishline = other.GetComponent<FinishLineComponent>();
        if (finishline != null)
        {
            var allCheckpoints = Resources.FindObjectsOfTypeAll<CheckpointComponent>();
            var numberOfCheckpoints = allCheckpoints.Count();
            // -1 because it starts at zero
            if (currentCheckpoint == numberOfCheckpoints - 1)
            {
                currentCheckpoint = 0;
                currentLap++;
                lapCounter.text = "Lap: " + (currentLap+1) + "/3";

                // time stuff
                lastLapTime = Mathf.FloorToInt(timeInCurrentLap);
                timeInCurrentLap = 0f;

                //Debug.Log("Lap Complete! Number of laps: " + currentLap);
            }
            return;
        }
    }

    // We gotta do this for terrain for some reason?
    void OnCollisionEnter(Collision collision)
    {
/*        Debug.Log("collision with " + collision.transform.name);
        if (collision.gameObject.transform.name.Contains("Agent"))
        {
            Debug.Log("marked collision with " + collision.transform.name);
            return;
        }*/

        TerrainCollider tc = collision.gameObject.GetComponent<TerrainCollider>();
        if (tc != null)
        {
            ResetToLastCheckpoint();
        }
    }

}
