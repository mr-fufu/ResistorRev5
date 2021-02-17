using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plating : MonoBehaviour {

    /* plating script (here bot plating is esssentially bot health) */

    // TODO SAM: Sync vars
    public int max_plating;
    public int current_plating;
    public bool destruction_trigger = false;

    // plating (health) bar object
    public Object plating_bar;

    // the amount of armor the bot has (each point of armor reduces incoming damage by 1 point)
    public int armor_value;
    private bool spawned;

    void Start()
    {
        // instantiate health bar object (UI display for bot plating/health) as a child
        // of current gameobject.
        Instantiate(plating_bar, gameObject.transform);

        // all other values are handled by the server only
            // check for the spawned bool to see whether the bot has been
            // put into play (legacy carryover from singleplayer)
        spawned = gameObject.GetComponent<AutoMove>().spawned;

        if (spawned)
        {
            // sets the max plating to the PLATE stat, the current plating equal to
            // the max plating and the armor_value equal to the ARMOR stat of the bot

            max_plating = gameObject.GetComponent<StandardStatBlock>().PLATE;
            current_plating = gameObject.GetComponent<StandardStatBlock>().PLATE;
            armor_value = gameObject.GetComponent<StandardStatBlock>().ARMOR;
        }
    }

    void Update()
    {
        // checks for destruction trigger. Handled only by the server,
        // if a bot has 0 plating or less, trigger that bot's destroy self script and 
        // destroy the gameobject this script is attached to across the server (i.e. for both clients)

        // also checks if current_plating exceeds max_plating of bot and if so then
        // resets the health to the maximum.

        if (spawned)
        {
            if (current_plating <= 0)
            {
                destruction_trigger = true;
                //Destroy(gameObject);
            }

            if (current_plating > max_plating)
            {
                current_plating = max_plating;
            }
        }
    }

    public void DamagePlating(int DamageInflicted)
    {
        // Simmilarly, damage is also handled only by the server. Reduce the incoming damage
        // value (passed by the projectile script) by the ARMOR stat up to a minimum of 1 incoming
        // damage. Then reduce the current plating by the remaining damage.

        if (spawned)
        {
            if (DamageInflicted > 1 && DamageInflicted > armor_value)
            {
                current_plating -= (DamageInflicted - armor_value);
            }
            else
            {
                current_plating -= 1;
            }
        }
    }

}
