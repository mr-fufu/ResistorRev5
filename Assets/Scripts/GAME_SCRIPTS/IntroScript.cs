using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class IntroScript : MonoBehaviour
{
    public GameObject slide0;
    public GameObject slide1;
    public GameObject slide2;
    public GameObject slide3;
    public GameObject slide4;
    public GameObject slide5;

    private GameObject[] slides;
    private int var;
    private Color slideColor;
    private Color fullTransparent;
    private Color fullOpaque;
    private float alpha;

    private float initialDelay;

    private float waitTime1;
    private float waitTime2;
    private float waitTime3;

    public GameObject loading_screen;
    public LoadingFade loading;

    public bool loaded;

    void Start()
    {

        loading = loading_screen.GetComponent<LoadingFade>();
        loading_screen.SetActive(false);

        alpha = 0;

        slideColor = new Vector4(1, 1, 1, 1);
        fullTransparent = new Vector4(1, 1, 1, 0);
        fullOpaque = new Vector4(1, 1, 1, 1);

        initialDelay = 0;

        waitTime1 = 0;
        waitTime2 = 0;
        waitTime3 = 0;

        var = 0;

        slides = new GameObject[6];

        slides[0] = slide0;
        slides[1] = slide1;
        slides[2] = slide2;
        slides[3] = slide3;
        slides[4] = slide4;
        slides[5] = slide5;

        for (int slide_count = 0; slide_count < 6; slide_count++)
        {
            slides[slide_count].GetComponent<SpriteRenderer>().color = fullTransparent;
        }

        loaded = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (slides[var].GetComponent<TypeWriter>().finish_typing != true)
            {
                waitTime1 = 0;
                waitTime2 = 0;
                waitTime3 = 0;

                slides[var].GetComponent<TypeWriter>().InstantType();
                Debug.Log("instant_typed");

                slides[var].GetComponent<SpriteRenderer>().color = fullOpaque;
            }
            else
            {
                waitTime3 = 1;
                Debug.Log("skiped_wait_time");
            }

            if (initialDelay < 1)
            {
                initialDelay = 1;
            }
        }

        if (initialDelay < 1)
        {
            initialDelay += Time.deltaTime * 5f;
        }
        else if (var < 5)
        {
            if (!slides[var].GetComponent<TypeWriter>().finish_typing)
            {
                if (alpha < 1)
                {
                    alpha += 0.01f;
                    slideColor.a = alpha;
                    slides[var].GetComponent<SpriteRenderer>().color = slideColor;
                }
                else if (slides[var].GetComponent<SpriteRenderer>().color != fullOpaque)
                {
                    slides[var].GetComponent<SpriteRenderer>().color = fullOpaque;
                }
                else
                {
                    if (waitTime1 < 1)
                    {
                        waitTime1 += Time.deltaTime * 5f;
                    }
                    else
                    {
                        if (!slides[var].GetComponent<TypeWriter>().start_typing)
                        {
                            slides[var].GetComponent<TypeWriter>().start_typing = true;
                        }
                    }
                }
            }
            else
            {
                if (waitTime2 < 1)
                {
                    waitTime2 += Time.deltaTime * 1f;
                }
                else
                {
                    if (alpha > 0)
                    {
                        alpha -= 0.01f;
                        slideColor.a = alpha;
                        slides[var].GetComponent<SpriteRenderer>().color = slideColor;
                    }
                    else if (slides[var].GetComponent<SpriteRenderer>().color != fullTransparent)
                    {
                        slides[var].GetComponent<SpriteRenderer>().color = fullTransparent;
                    }
                    else
                    {
                        if (waitTime3 < 1)
                        {
                            waitTime3 += 10f * Time.deltaTime;
                        }
                        else
                        {
                            var++;
                            waitTime1 = 1.0f;
                            waitTime3 = 0;
                        }
                    }
                }
            }
        }
        else if (!loaded)
        {
            loaded = true;
            StartCoroutine(LoadWithScreen("LobbyScene"));
        }
    }

    private IEnumerator LoadWithScreen(string load_destination)
    {
        loading_screen.SetActive(true);
        loading.FadeInLoadScreen();

        yield return new WaitUntil(() => loading.fadeInComplete == true);
        PhotonNetwork.LoadLevel(load_destination);
    }
}