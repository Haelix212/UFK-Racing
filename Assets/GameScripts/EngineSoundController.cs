using UnityEditor;
using UnityEngine;

public class EngineSoundController : MonoBehaviour
{
    public AudioSource accelerationSource;
    public float maxPitch; // Maximum pitch value for acceleration
    public float startPitch; // starting pitch
    private KartController kartController; // Reference to your kart's controller script
    private float pitchDecay = 0.98f;
    private int idleTime = 0;

    void Start()
    {
        // Get the kart controller component
        kartController = GetComponent<KartController>();

        // Start playing the idle sound
        accelerationSource.Play();
    }

    void Update()
    {
        // Get the current speed of the kart from the kart controller
        float speed = kartController.GetCurrentSpeed();

        // Control the pitch of the acceleration sound based on the speed
        if (Mathf.Abs(speed) <= 0.01f) {
            if (idleTime > 120) {
                accelerationSource.pitch = Mathf.Max(accelerationSource.pitch * pitchDecay, 0f);
            } else {
                idleTime++;
            }
        } else {
            accelerationSource.pitch = startPitch + (speed / kartController.topSpeed) * (maxPitch - 1.0f);
            idleTime = 0;
        }
    }
}
