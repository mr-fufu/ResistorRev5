using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LoadingFade : MonoBehaviour
{
    public bool fade_in;
    public bool load_complete;

    public float alpha = 0;
    public Color fade_color = new Vector4(1, 1, 1, 1);
    public Color transparency = new Vector4(1, 1, 1, 1);

    private int child_count;
    public bool fading;
    public bool fade_state;

    public int fade_delay;

    public bool loaded_in;

    // Start is called before the first frame update
    void Start()
    {
        fade_color = GetComponent<SpriteRenderer>().color;

        fade_delay = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (!fading)
        {
            if (fade_in)
            {
                fade_state = true;
                fading = true;
            }
            else
            {
                fade_state = false;
                fading = true;
            }
        }

        if (fade_state)
        {
            if(PhotonNetwork.LevelLoadingProgress > 0.96)
            {
                alpha = 1;
                fade_color.a = 1;
                GetComponent<SpriteRenderer>().color = fade_color;
                fading = false;
                loaded_in = true;
            }
            else
            {
                fade_color.a = alpha;
                alpha += 0.01f;
                alpha = PhotonNetwork.LevelLoadingProgress;
                GetComponent<SpriteRenderer>().color = fade_color;
            }
        }
        else
        {
            if (alpha > 0)
            {
                if (fade_delay > 0)
                {
                    fade_delay--;
                }
                else
                {
                    fading = true;
                    fade_color.a = alpha;
                    alpha -= 0.006f;
                    GetComponent<SpriteRenderer>().color = fade_color;
                }
            }
            else
            {
                alpha = 0;
                fade_color.a = 0;
                GetComponent<SpriteRenderer>().color = fade_color;
                fading = false;
            }
        }

        transparency.a = fade_color.a;
        change_children(transparency);

        if ((alpha == 0) && load_complete)
        {
            Destroy(gameObject);
        }
    }

    private void change_children(Color set_color)
    {
        child_count = gameObject.transform.childCount;

        for (int var = 0; var < child_count; var++)
        {
            gameObject.transform.GetChild(var).gameObject.GetComponent<SpriteRenderer>().color = set_color;
        }
    }
}
