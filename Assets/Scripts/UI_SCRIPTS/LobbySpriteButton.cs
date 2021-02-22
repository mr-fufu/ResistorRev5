using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySpriteButton : MonoBehaviour
{
    public bool multiplayer_lobby;
    public bool host_lobby;

    public Sprite alt_sprite;
    private Sprite reg_sprite;

    public bool use_light;
    public bool hide_sprite;
    public GameObject forward_light;
    public GameObject reverse_light;

    public bool use_spawn_block;
    public GameObject spawn_block;

    void Start()
    {
        reg_sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        if (use_light)
        {
            forward_light.SetActive(false);
            reverse_light.SetActive(false);
        }
        if (hide_sprite)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Vector4(0, 0, 0, 0);
        }
    }

    public void OnMouseDown()
    {
        /*if (multiplayer_lobby)
        {
            gameObject.transform.parent.GetComponent<CustomHost>().JoinMatch();
        }
        else if (host_lobby)
        {
            gameObject.transform.parent.GetComponent<CustomLobbyPlayer>().OnClickJoin();
        }*/
    }

    public void OnMouseEnter()
    {
        if (use_spawn_block)
        {
            if (!spawn_block.GetComponent<SpawnBlocker>().spawnBlocked)
            {
                if (!hide_sprite)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = alt_sprite;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);
                }

                if (use_light)
                {
                    forward_light.SetActive(true);
                    reverse_light.SetActive(true);
                }
            }
        }
        else
        {
            if (!hide_sprite)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = alt_sprite;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);
            }

            if (use_light)
            {
                forward_light.SetActive(true);
                reverse_light.SetActive(true);
            }

        }
    } 

    public void OnMouseExit()
    {
        if (!hide_sprite)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = reg_sprite;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 0);
        }

        if (use_light)
        {
            forward_light.SetActive(false);
            reverse_light.SetActive(false);
        }
    }
}
