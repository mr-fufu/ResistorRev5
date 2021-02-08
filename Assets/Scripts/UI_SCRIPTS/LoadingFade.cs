using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LoadingFade : MonoBehaviour
{
    public bool fadeInComplete;

    public float alpha = 0;
    private Color fadeColor = new Vector4(1, 1, 1, 1);
    private Color childColor = new Vector4(1, 1, 1, 1);

    private int childCount;
    public bool fadeHold;
    public bool setVisible;

    private int fadeDelay;

    Scene currentScene;

    void Start()
    {
        fadeHold = false;
        fadeColor = GetComponent<SpriteRenderer>().color;
        DontDestroyOnLoad(gameObject.transform.parent.gameObject);

        currentScene = SceneManager.GetActiveScene();
        setVisible = false;
        fadeInComplete = false;

        fadeColor.a = 0;
        GetComponent<SpriteRenderer>().color = fadeColor;
        childColor.a = fadeColor.a;
        ChangeChildren(childColor);
    }

    void Update()
    {
        // code below fades alpha in/out based on set_visible bool
        if (fadeHold != setVisible)
        {
            if (setVisible)
            {
                if (alpha >= 1)
                {
                    alpha = 1;
                    fadeHold = true;
                    fadeInComplete = true;
                }
                else
                {
                    alpha += 0.005f;
                }
            }
            else
            {
                if (alpha < 0)
                {
                    alpha = 0;
                    fadeHold = false;
                }
                else
                {
                    if (fadeDelay > 0)
                    {
                        fadeDelay--;
                    }
                    else
                    {
                        alpha -= 0.002f;
                    }
                }
            }

            // if currently fading, change color of sprite and color of child sprites to match fade_color's transparency
            // otherwise, if fading in is complete and scene has changed, fade back out (to fully transparent)
            fadeColor.a = alpha;
            GetComponent<SpriteRenderer>().color = fadeColor;
            childColor.a = alpha;
            ChangeChildren(childColor);
        }
        else
        {
            if (SceneManager.GetActiveScene() != currentScene)
            {
                gameObject.transform.parent.GetComponent<Canvas>().worldCamera = Camera.main;
                //current_scene = SceneManager.GetActiveScene();
                setVisible = false;
            }
        }

        // if scene has changed and object has faded out, destroy self
        if ((alpha == 0) && fadeInComplete)
        {
            Destroy(gameObject.transform.parent.gameObject);
            Destroy(gameObject);
        }
    }

    // method to initiate a scene change. Calling script should then check fade_complete == true i.e. when
    // this object has fully faded in before changing scenes
    public void FadeInLoadScreen()
    {
        setVisible = true;
        fadeDelay = 100;
    }

    private void ChangeChildren(Color setColor)
    {
        childCount = gameObject.transform.childCount;

        for (int var = 0; var < childCount; var++)
        {
            gameObject.transform.GetChild(var).gameObject.GetComponent<SpriteRenderer>().color = setColor;
        }
    }
}
