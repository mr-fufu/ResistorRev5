using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkColor : NetworkBehaviour
{
    public GameObject color_object;
    public string search_text;
    [SyncVar] public GameObject leg_component;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PartStats>().part_type == "LEG")
        {
            if (search_text != "Player1Color" && search_text != "Player1Color")
            {
                if (gameObject.tag == "BOT_Player")
                {
                    search_text = "Player1Color";
                }
                else if (gameObject.tag == "BOT_Enemy")
                {
                    search_text = "Player2Color";
                }
            }
            else
            {
                if (color_object != null)
                {
                    gameObject.GetComponent<SpriteRenderer>().color = color_object.GetComponent<SpriteRenderer>().color;
                }
                else
                {
                    color_object = GameObject.Find(search_text);
                }
            }
        }
        else
        {
            find_leg_object(gameObject);

            gameObject.GetComponent<SpriteRenderer>().color = leg_component.GetComponent<SpriteRenderer>().color;
        }
    }

    void find_leg_object(GameObject find_object)
    {
        if (find_object.GetComponent<PartStats>() != null)
        {
            if (find_object.GetComponent<PartStats>().part_type == "LEG")
            {
                leg_component = find_object;
            }
            else
            {
                if (find_object.transform.parent != null)
                {
                    if (find_object.transform.parent.parent != null)
                    {
                        find_leg_object(find_object.transform.parent.parent.gameObject);
                    }
                    else
                    {
                        leg_component = null;
                    }
                }
                else
                {
                    leg_component = null;
                }
            }
        }
        else
        {
            leg_component = null;
        }
    }
}
