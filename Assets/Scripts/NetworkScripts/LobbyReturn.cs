using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyReturn : MonoBehaviour
{
    public GameObject network_manager;

    public Sprite alt_sprite;
    public Sprite click_sprite;
    private Sprite reg_sprite;

    public bool host;

    private int sibling_count;

    void Start()
    {
        reg_sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    public void OnMouseDown()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = click_sprite;

        gameObject.GetComponent<SpriteRenderer>().sprite = reg_sprite;

        network_manager.GetComponent<CustomNetworkManager>().BackToMenu(host);
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
