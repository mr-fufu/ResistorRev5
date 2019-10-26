using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkBotColor : NetworkBehaviour
{
    public GameObject color_object;
    public string search_text;
    public GameObject leg_component;

    [SyncVar] public bool alt;
    [SyncVar] public int no;

    public Vector4[] spawn_color = new Vector4[4];
    public Vector4[] spawn_color_alt = new Vector4[4];

    // Start is called before the first frame update
    void Start()
    {
        spawn_color[0] = new Vector4(1f, 0.59f, 0.3f, 1f);
        spawn_color[1] = new Vector4(0.6f, 0.85f, 0.24f, 1f);
        spawn_color[2] = new Vector4(0.1f, 0.77f, 1f, 1f);
        spawn_color[3] = new Vector4(0.34f, 1f, 0.95f, 1f);

        spawn_color_alt[0] = new Vector4(1f, 0.2f, 0.69f, 1f);
        spawn_color_alt[1] = new Vector4(1f, 0.95f, 0.38f, 1f);
        spawn_color_alt[2] = new Vector4(1f, 0.29f, 0.29f, 1f);
        spawn_color_alt[3] = new Vector4(0.85f, 0.94f, 0.97f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            if (search_text != "Player1Color" && search_text != "Player2Color")
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
                    alt = color_object.GetComponent<BotColorObject>().alt;
                    no = color_object.GetComponent<BotColorObject>().no;
                }
                else
                {
                    color_object = GameObject.Find(search_text);
                }
            }

            if (GetComponent<PartStats>().part_type != "LEG")
            {
                find_leg_object(gameObject);

                if (leg_component != null)
                {
                    alt = leg_component.GetComponent<NetworkBotColor>().alt;
                    no = leg_component.GetComponent<NetworkBotColor>().no;
                    //gameObject.GetComponent<SpriteRenderer>().color = leg_component.GetComponent<SpriteRenderer>().color;
                }
            }
        }

        if (alt)
        {
            GetComponent<SpriteRenderer>().color = spawn_color_alt[no];
        }
        else
        {
            GetComponent<SpriteRenderer>().color = spawn_color[no];
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
