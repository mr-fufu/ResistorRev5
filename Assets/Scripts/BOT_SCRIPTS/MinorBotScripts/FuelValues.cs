using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelValues : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider bar_value;
    public int fuel_remaining;

    // Use this for initialization
    void Start()
    {
        bar_value.maxValue = 100;
    }

    // Update is called once per frame
    void Update()
    {
        fuel_remaining = transform.parent.gameObject.GetComponent<Fueling>().fuel_remaining;

        bar_value.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 8, gameObject.transform.position.z);

        bar_value.value = fuel_remaining;
    }
}
