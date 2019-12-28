using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SP_BotColorObject : NetworkBehaviour
{
    [SyncVar] public bool alt;
    [SyncVar] public int no;

    public Vector4[] spawn_color = new Vector4[4];
    public Vector4[] spawn_color_alt = new Vector4[4];

    [SyncVar] public bool enemy;

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
