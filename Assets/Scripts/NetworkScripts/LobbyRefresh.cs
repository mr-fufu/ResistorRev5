using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyRefresh : MonoBehaviour
{
    public GameObject network_manager;

    public Sprite alt_sprite;
    public Sprite click_sprite;
    private Sprite reg_sprite;

    private int sibling_count;

    void Start()
    {
        reg_sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    public void OnMouseDown()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = click_sprite;

        sibling_count = gameObject.transform.parent.childCount;

        for (int sibling = 0; sibling < sibling_count; sibling++)
        {
            if (gameObject.transform.parent.GetChild(sibling).tag != "MenuClickable")
            {
                Destroy(gameObject.transform.parent.GetChild(sibling).gameObject);
            }
        }

        network_manager.GetComponent<CustomMatchmakingLobby>().PopulateLobby();

        gameObject.GetComponent<SpriteRenderer>().sprite = reg_sprite;
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
