using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BotColorObject : NetworkBehaviour
{
    [SyncVar] public bool alt;
    [SyncVar] public int no;

    private Vector4[] spawn_color = new Vector4[4];
    private Vector4[] spawn_color_alt = new Vector4[4];

    public bool enemy;

    // Start is called before the first frame update
    void Start()
    {
        spawn_color[0] = new Vector4(1f, 0.9f, 0.6f, 1);
        spawn_color[1] = new Vector4(0.3f, 0.5f, 0.3f, 1);
        spawn_color[2] = new Vector4(0.1f, 0.5f, 0.6f, 1);
        spawn_color[3] = new Vector4(0.5f, 0.9f, 0.8f, 1);

        spawn_color_alt[0] = new Vector4(0.7f, 0, 0.3f, 1);
        spawn_color_alt[1] = new Vector4(0.3f, 0.25f, 0.15f, 1);
        spawn_color_alt[2] = new Vector4(0.82f, 0.3f, 0.3f, 1);
        spawn_color_alt[3] = new Vector4(0.6f, 0.6f, .9f, 1);

        if (alt)
        {
            GetComponent<SpriteRenderer>().color = spawn_color_alt[no];
        }
        else
        {
            GetComponent<SpriteRenderer>().color = spawn_color[no];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy)
        {
            gameObject.name = "Player2Color";
        }
        else
        {
            gameObject.name = "Player1Color";
        }
    }
}
