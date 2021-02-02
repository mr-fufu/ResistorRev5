using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LoadingFade : MonoBehaviour
{
    public bool fade_complete;

    public float alpha = 0;
    public Color fade_color = new Vector4(1, 1, 1, 1);
    public Color transparency = new Vector4(1, 1, 1, 1);

    private int child_count;
    public bool fading;
    public bool set_visible;

    public int fade_delay;

    public bool loaded_in;
    Scene current_scene;

    void Start()
    {
        fade_color = GetComponent<SpriteRenderer>().color;
        DontDestroyOnLoad(gameObject);

        current_scene = SceneManager.GetActiveScene();
        set_visible = false;
        fade_complete = false;
    }

    void Update()
    {
        // code below fades alpha in/out based on set_visible bool
        if (set_visible)
        {
            if(alpha > 1)
            {
                alpha = 1;
                fade_color.a = 1;

                fading = false;
                loaded_in = true;

                fade_complete = true;
            }
            else
            {
                fade_color.a = alpha;
                alpha += 0.01f;
                GetComponent<SpriteRenderer>().color = fade_color;
            }
        }
        else
        {
            if (alpha < 0)
            {
                alpha = 0;
                fade_color.a = 0;
                GetComponent<SpriteRenderer>().color = fade_color;
                fading = false;
            }
            else
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
        }

        // if currently fading, change color of sprite and color of child sprites to match fade_color's transparency
        // otherwise, if fading in is complete and scene has changed, fade back out (to fully transparent)
        if (fading)
        {
            GetComponent<SpriteRenderer>().color = fade_color;
            transparency.a = fade_color.a;
            change_children(transparency);

        }
        else
        {
            if (SceneManager.GetActiveScene() != current_scene)
            {
                //current_scene = SceneManager.GetActiveScene();
                set_visible = false;
            }
        }

        // if scene has changed and object has faded out, destroy self
        if ((alpha == 0) && fade_complete)
        {
            Destroy(gameObject);
        }
    }

    // method to initiate a scene change. Calling script should then check fade_complete == true i.e. when
    // this object has fully faded in before changing scenes
    public void change_scene()
    {
        set_visible = true;
        fade_delay = 100;
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
