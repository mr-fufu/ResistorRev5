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

    void Start()
    {
        reg_sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    public void OnMouseDown()
    {
        if (multiplayer_lobby)
        {
            gameObject.transform.parent.GetComponent<CustomHost>().JoinMatch();
        }
        else if (host_lobby)
        {
            gameObject.transform.parent.GetComponent<CustomLobbyPlayer>().OnClickJoin();
        }
    }

    public void OnMouseEnter()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = alt_sprite;
    }

    public void OnMouseExit()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = reg_sprite;
    }
}
