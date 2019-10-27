using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkColor : MonoBehaviour
{
    public bool enemy;
    public bool recolor_light;
    public bool sprite;
    public bool text;

    public string search_text;
    public GameObject color_object;

    public bool colored;
    private Color hold_color;

    // Start is called before the first frame update
    void Start()
    {
        colored = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!colored)
        {
            if (search_text != "Player1Color" && search_text != "Player2Color")
            {
                if (enemy)
                {
                    search_text = "Player2Color";
                }
                else
                {
                    search_text = "Player1Color";
                }
            }
            else
            {
                if (color_object != null)
                {
                    if (recolor_light)
                    {
                        GetComponent<Light>().color = color_object.GetComponent<SpriteRenderer>().color;
                    }

                    if (sprite)
                    {
                        //hold_color = color_object.GetComponent<SpriteRenderer>().color;
                        //GetComponent<SpriteRenderer>().color = new Vector4(hold_color.x, hold_color.y, hold_color.z, GetComponent<SpriteRenderer>().color.a);

                        hold_color = color_object.GetComponent<SpriteRenderer>().color;
                        hold_color.a = GetComponent<SpriteRenderer>().color.a;
                        GetComponent<SpriteRenderer>().color = hold_color;
                    }

                    if (text)
                    {
                        GetComponent<Text>().color = color_object.GetComponent<SpriteRenderer>().color;
                    }

                    colored = true;
                }
                else
                {
                    color_object = GameObject.Find(search_text);
                }
            }
        }
    }
}
