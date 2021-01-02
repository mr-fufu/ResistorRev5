using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fueling : MonoBehaviour
{
    // TODO SAM: sync vars
    public bool fuel_used;
    public int fuel_remaining;
    private float fuel_clock;

    public bool fuel_instantiated;

    private int fuel;

    public GameObject fuel_bar;

    private bool spawned;

    void Start()
    {
        // check whether the bot has spawned (Fueling script attached to 
        // leg part which always contains the automove script)
        spawned = gameObject.GetComponent<AutoMove>().spawned;

        if (spawned)
        {
            // start with the fuel value equal to the FUEL stat (affects how quickly fuel
            // recovers) fuel_remaining starts at 100
            fuel = gameObject.GetComponent<StandardStatBlock>().FUEL;

            fuel_remaining = 100;
        }
    }

    void Update()
    {
        // if the bot has a component that uses fuel, the projectile attack script will
        // find the leg part and enable fuel_used
        if (fuel_used)
        {
            // Instantiate the fuel object a single time
            if (!fuel_instantiated)
            {
                fuel_instantiated = true;
                Instantiate(fuel_bar, gameObject.transform);
            }

            // if the fuel is not max, increase the fuel clock by the fuel stat of the bot. fuel clock counts to 100 and
            // increases the fuel remaining by 1 and is then reset
            if (fuel_remaining < 100)
            {
                fuel_clock += Time.deltaTime * fuel * 100f;
                if (fuel_clock > 100)
                {
                    fuel_remaining++;
                    fuel_clock = 0;
                }
            }
            else if (fuel_remaining > 100)
            {
                fuel_remaining = 100;
            }
        }
    }

    // only the server runs this function, fuel remaining is increased by a transmitted amount (usually negative, representing
    // fuel used in projectile attack). Function is called by projectile attack when projectile is spawned
    public void transmit_fuel(int fuel_transmitted)
    {
        if (spawned)
        {
            fuel_remaining += fuel_transmitted;
        }
    }
}
