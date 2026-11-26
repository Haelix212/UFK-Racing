using UnityEngine;
using System.Collections;
using TMPro;

//Wissam El Hajj, 2016
//this is a simple script that creates a carousel form game objects
//this script is provided as is without any guarentees
/*
* to use this script just place it on any gameobject in the scene, this element shouldn't have a collider so an empty game object is ideal
* add elements to the carouselObjects array, make sure these items have a collider
* make sure there are no elements with colliders in between the center and the carousel elements
*/
public class CarCarousel : MonoBehaviour
{

    public GameObject circlePlacement;
    public Vector3 displayLocation = Vector3.zero;
    public GameObject[] carouselObjects;//the elements of the carousel
    public bool ResetCenterRotation = true;//do you want to reset the rotation of the carousel center (recommended to be true)
    public float DistanceFromCenter = 10.0f;//the distance from the center of the carousel
    public bool AssumeObject = true; // if true assume the object that is picked, otherwise (false) keep checking what the next item is through raycast.
    public static int ChosenCar = 0; //index of the object that is centered in the carousel
    public float speedOfrotation = 0.1f; //the speed in which the carousel rotates: values should be between 0.01f -> 1.0f, zero will stop the rotation


    private static float diameter = 360.0f; //the diameter is always 360 degrees
    private Transform theRayCaster = null; //create an empty transform
    private float Angle = 0.0f; //the angle for each object in the carousel
    private float newAngle = 0.0f; //the calculated angle
    private bool firstTime = true; //used to calculate the offset for the first time
    GameObject carTitleObject;

    void Start()
    {
        ChosenCar = 0;
        Debug.Log("circle " + circlePlacement.transform.localPosition);
        displayLocation = circlePlacement.transform.localPosition;
        // Debug.Log("got a child? " + carouselObjects.Length);
        // Debug.Log(carouselObjects[0].name);//just display the name of the first chosen element in the console
        GameObject raycastHolder = new GameObject();//create an empty gameobject
        raycastHolder.name = "RaycastPicker"; //rename it to RaycastPicker
        theRayCaster = raycastHolder.transform; // assign the transform of the newlycreated gameobject to the raycast transform variable
        theRayCaster.position = transform.position; // place it at the positon of the carousel center
        if (ResetCenterRotation)
        {
            transform.rotation = Quaternion.identity;//reset the rotation of the carousel center
        }

        Angle = diameter / (float)carouselObjects.Length;//calculate the angle according to the number of elements

        carTitleObject = GameObject.Find("Car title");
        if (carTitleObject != null)
        {
            // Get the TextMeshProUGUI component and set its text to the name of the chosen car
            TextMeshProUGUI carTitleText = carTitleObject.GetComponent<TextMeshProUGUI>();
            if (carTitleText != null)
            {
                carTitleText.text = carouselObjects[0].name;
            }
        }

        carouselObjects[0].transform.localPosition = this.displayLocation;//Reset objects to the postion of the carousel center
        carouselObjects[0].transform.parent = this.transform; // make the element child to the carousel center
        /*carouselObjects[0].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);*/

        for (int i = 1; i < carouselObjects.Length; i++)
        { //loop through the objects
            
            carouselObjects[i].transform.position = new Vector3(-100f, -100f, -100f);//Reset objects to the postion of the carousel center
            carouselObjects[i].transform.parent = this.transform; // make the element child to the carousel center
            /*carouselObjects[i].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);*/
        }
    }

    // Update is called once per frame
    void Update()
    {
        carouselObjects[ChosenCar].transform.Rotate(new Vector3(0, 30, 0) * 0.75f * Time.deltaTime);
    }

    public void rotateTheCarouselLeft() // call this function to rotate the carousel towards the left
    {
        Debug.Log("rotating left");
        if (firstTime)// if run the first time calcule the offset
        {
            newAngle = transform.eulerAngles.y;
            newAngle += Angle;
            firstTime = false; // stop this piece of code from running in the future
        }
        else
        {
            newAngle += Angle; //calculate the new angle
        }

        carouselObjects[ChosenCar].transform.position = new Vector3(-100, -100, -100);//Reset objects to the postion of the carousel center
        
        if (AssumeObject == true)//here we check which element is selected and if we reached the start of the array we reset the index
        {
            if (ChosenCar <= 0)
            {
                ChosenCar = carouselObjects.Length - 1;
            }
            else
            {
                ChosenCar--;
            }
            Debug.Log(carouselObjects[ChosenCar].name); //show in the console the name of the selected element
            carouselObjects[ChosenCar].transform.localPosition = this.displayLocation;//Reset objects to the postion of the carousel center
            carouselObjects[ChosenCar].transform.parent = this.transform; // make the element child to the carousel center            
        }

        // Get the TextMeshProUGUI component and set its text to the name of the chosen car
        TextMeshProUGUI carTitleText = carTitleObject.GetComponent<TextMeshProUGUI>();
        if (carTitleText != null)
        {
            carTitleText.text = carouselObjects[ChosenCar].name;
        }
    }
    public void rotateTheCarouselRight()// call this function to rotate the carousel towards the right
    {

        Debug.Log("rotating right");
        if (firstTime) // if run the first time calcule the offset
        {
            newAngle = transform.eulerAngles.y;
            newAngle -= Angle;
            firstTime = false; // stop this piece of code from running in the future
        }
        else
        {
            newAngle -= Angle; //calculate the new angle
        }

        carouselObjects[ChosenCar].transform.position = new Vector3(-100, -100, -100);//Reset objects to the postion of the carousel center

        if (AssumeObject == true) //here we check which element is selected and if we reached the end of the array we reset the index
        {
            if (ChosenCar >= carouselObjects.Length - 1)
            {
                ChosenCar = 0;
            }
            else
            {
                ChosenCar++;
            }
            Debug.Log(carouselObjects[ChosenCar].name); //show in the console the name of the selected element
            carouselObjects[ChosenCar].transform.localPosition = this.displayLocation;//Reset objects to the postion of the carousel center
            carouselObjects[ChosenCar].transform.parent = this.transform; // make the element child to the carousel center

        }

        TextMeshProUGUI carTitleText = carTitleObject.GetComponent<TextMeshProUGUI>();
        if (carTitleText != null)
        {
            carTitleText.text = carouselObjects[ChosenCar].name;
        }
    }
}
