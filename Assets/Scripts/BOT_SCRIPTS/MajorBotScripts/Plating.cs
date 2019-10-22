using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Plating : NetworkBehaviour {

    // plating script (here bot plating is esssentially bot health)
    // syncVars are values that are updated across the server when they change
    // and are handled by the client. I.e. SyncVars can change on non-server or server
    // clients and both clients will update the value

    [SyncVar] public int max_plating;
    [SyncVar] public int current_plating;
    [SyncVar] public bool destruction_trigger = false;

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
        if (isServer)
        {
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
    }

    void Update()
    {
        // checks for destruction trigger. Handled only by the server,
        // if a bot has 0 plating or less, trigger that bot's destroy self script and 
        // destroy the gameobject this script is attached to across the server (i.e. for both clients)

        // also checks if current_plating exceeds max_plating of bot and if so then
        // resets the health to the maximum.

        if (isServer)
        {
            if (spawned)
            {
                if (current_plating <= 0)
                {
                    destruction_trigger = true;
                    NetworkServer.Destroy(gameObject);
                }

                if (current_plating > max_plating)
                {
                    current_plating = max_plating;
                }
            }
        }
    }

    public void DamagePlating(int DamageInflicted)
    {
        // Simmilarly, damage is also handled only by the server. Reduce the incoming damage
        // value (passed by the projectile script) by the ARMOR stat up to a minimum of 1 incoming
        // damage. Then reduce the current plating by the remaining damage.

        if (isServer)
        {
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

    public void RepairPlating(int RepairInflicted)
    {
        // currently not implemented, similar to damage only 
        if (isServer)
        {
            if (spawned)
            {
                if ((RepairInflicted + current_plating) > max_plating)
                {
                    current_plating = max_plating;
                }
                else
                {
                    current_plating += RepairInflicted;
                }
            }
        }
    }
}
