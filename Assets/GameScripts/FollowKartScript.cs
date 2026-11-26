using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowKartScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject kart;
    public GameObject indicator;
    void Start()
    {
        if (CarCarousel.ChosenCar == 0)
        {
            // kart 1 - F1
            kart = GameObject.Find("player_kart_1");
        }
        else if (CarCarousel.ChosenCar == 1)
        {
            // kart 2 - Monster Truck
            kart = GameObject.Find("player_kart_2");
        }
        else if (CarCarousel.ChosenCar == 2)
        {
            // kart 3 - Truck
            kart = GameObject.Find("player_kart_3");
        }
        else
        {
            // kart 4 - Delorean
            kart = GameObject.Find("player_kart_4");
        }
    }

    // Update is called once per frame
    void Update()
    {
        var x = kart.transform.position.x;
        var y = indicator.transform.position.y;
        var z = kart.transform.position.z;
        
        indicator.transform.position = new Vector3(x, y, z);

    }
}
