﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PortraitManager : NetworkBehaviour
{
    // minor script: changes the profile picture by swapping sprites of the sprite renderer

    public Sprite profile1;
    public Sprite profile2;
    public Sprite profile3;
    public Sprite profile4;

    public Sprite alt_profile1;
    public Sprite alt_profile2;
    public Sprite alt_profile3;
    public Sprite alt_profile4;

    [SyncVar] public int profile_no;
    [SyncVar] public bool alt;
    private Sprite[] profile;
    private Sprite[] alt_profile;

    public bool inc;
    public bool dec;

    // Start is called before the first frame update
    void Start()
    {
        // start at default profile 0 (non-alt artwork)
        // makes 2 arrays profile and alt profile of size 4 each and
        // sets the appropriate sprites in each index

        profile_no = 0;

        profile = new Sprite[4];
        alt_profile = new Sprite[4];

        profile[0] = profile1;
        profile[1] = profile2;
        profile[2] = profile3;
        profile[3] = profile4;

        alt_profile[0] = alt_profile1;
        alt_profile[1] = alt_profile2;
        alt_profile[2] = alt_profile3;
        alt_profile[3] = alt_profile4;
    }

    // Update is called once per frame
    void Update()
    {
        // update the sprite of the sprite renderer based on the array of the profile
        // or alt-profile based on the bool alt

        if (alt)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = alt_profile[profile_no];
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = profile[profile_no];
        }

        if (gameObject.transform.parent.gameObject.GetComponent<NetworkIdentity>() != null)
        {
            if (gameObject.transform.parent.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                GetComponent<PortraitManager>().alt = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>().port_alt;
                GetComponent<PortraitManager>().profile_no = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>().port_no;
            }
        }
    }

    // 3 public functions to increase or decrease the profile number or to toggle alt. Cycles through
    // profile numbers from 0 1 2 3 (there's probably a more efficient built in way to do this but I just
    // ran through it I mean fk it it works
    public void IncProfile()
    {
        if (profile_no < 3)
        {
            profile_no++;
        }
        else
        {
            profile_no = 0;
        }
    }

    public void DecProfile()
    {
        if (profile_no > 0)
        {
            profile_no--;
        }
        else
        {
            profile_no = 3;
        }
    }

    public void ToggleAlt()
    {
        if (alt)
        {
            alt = false;
        }
        else
        {
            alt = true;
        }
    }
}
