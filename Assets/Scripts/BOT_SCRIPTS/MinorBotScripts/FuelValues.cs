using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelValues : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider bar_value;
    public int fuel_remaining;
    private Fueling fuelingOnLegs;

    // Use this for initialization
    void Start()
    {
        bar_value.maxValue = 100;
        fuelingOnLegs = transform.parent.gameObject.GetComponent<Fueling>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fuelingOnLegs != null)
        {
            fuel_remaining = fuelingOnLegs.fuel_remaining;
        }
        else
        {
            Destroy(gameObject);
        }

        bar_value.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 8, gameObject.transform.position.z);

        bar_value.value = fuel_remaining;
    }
}
