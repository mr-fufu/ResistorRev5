using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Object = UnityEngine.Object;

public class Plating : MonoBehaviourPunCallbacks
{

    /* plating script (here bot plating is essentially bot health) */

    // TODO SAM: Sync vars
    public int max_plating;
    public int current_plating;
    public bool destruction_trigger = false;

    // plating (health) bar object
    public Object platingBarPrefab;
    public GameObject platingBar;

    // the amount of armor the bot has (each point of armor reduces incoming damage by 1 point)
    public int armor_value;

    private void Awake()
    {
        current_plating = 1;
    }

    void Start()
    {
        // instantiate health bar object (UI display for bot plating/health) as a child
        // of current gameobject.
        /*
         * if (GetComponent<AutoMove>()?.enemy_check != PhotonNetwork.IsMasterClient)
        {
            platingBar = PhotonNetwork.Instantiate("ParticlesAndEffects/" + platingBarPrefab.name, gameObject.transform.position, gameObject.transform.rotation);
            platingBar.transform.parent = gameObject.transform;
        }
        */

        platingBar = (GameObject)Instantiate(platingBarPrefab, gameObject.transform);
    }

    public void InitializePlating(int plate, int armor)
    {
        Debug.Log("Initializing Plating");
        max_plating = plate;
        current_plating = plate;
        armor_value = armor;
    }

    void Update()
    {
        // checks for destruction trigger. Handled only by the server,
        // if a bot has 0 plating or less, trigger that bot's destroy self script and 
        // destroy the gameobject this script is attached to across the server (i.e. for both clients)

        // also checks if current_plating exceeds max_plating of bot and if so then
        // resets the health to the maximum.

        if (current_plating <= 0)
        {
            destruction_trigger = true;
            if (GetComponent<AutoMove>()?.enemy_check != PhotonNetwork.IsMasterClient)
            {
                Destroy(platingBar);
                GetComponent<PartStats>().deconstruct();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        if (current_plating > max_plating)
        {
            current_plating = max_plating;
        }
    }

    public void DamagePlating(int DamageInflicted)
    {
        // Simmilarly, damage is also handled only by the server. Reduce the incoming damage
        // value (passed by the projectile script) by the ARMOR stat up to a minimum of 1 incoming
        // damage. Then reduce the current plating by the remaining damage.

        if (GetComponent<StandardStatBlock>().ENEMY != PhotonNetwork.IsMasterClient)
        {
            if (DamageInflicted > 1 && DamageInflicted > armor_value)
            {
                current_plating -= (DamageInflicted - armor_value);
            }
            else
            {
                current_plating -= 1;
            }

            photonView.RPC("SyncPlating", RpcTarget.Others, current_plating);
        }

        //Debug.LogError("new:" + current_plating );
    }

    [PunRPC]
    public void SyncPlating(int newPlating)
    {
        current_plating = newPlating;
    }
}
