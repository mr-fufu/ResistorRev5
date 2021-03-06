﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class IntroScript : MonoBehaviour
{
    public GameObject fade_black;
    private bool initial_fade;
    
    public GameObject slide1;
    public GameObject slide2;
    public GameObject slide3;
    public GameObject slide4;
    public GameObject slide5;
    public GameObject slide6;

    private GameObject[] slides;
    private int var;
    private Color slide_color;
    private Color full_transparent;
    private float alpha;

    private float transition_time;
    private float wait_time;

    public GameObject loading_screen;
    public LoadingFade loading;

    private bool in_load;

    private bool set_load;
    private string load_location;
    private bool currently_loading;

    // Start is called before the first frame update
    void Start()
    {
        loading = loading_screen.GetComponent<LoadingFade>();
        loading_screen.SetActive(false);

        alpha = 1;

        slide_color = new Vector4(0, 0, 0, 1);
        full_transparent = new Vector4(1, 1, 1, 0);

        transition_time = 0;
        wait_time = 0;

        var = 0;

        slides = new GameObject[6];

        slides[0] = slide1;
        slides[1] = slide2;
        slides[2] = slide3;
        slides[3] = slide4;
        slides[4] = slide5;
        slides[5] = slide6;

        for (int slide_count = 0; slide_count < 6; slide_count++)
        {
            slides[slide_count].GetComponent<SpriteRenderer>().color = full_transparent;
        }

        slides[0].GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);
        fade_black.SetActive(true);
        fade_black.GetComponent<SpriteRenderer>().color = new Vector4(0, 0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!in_load)
        {

            if (!initial_fade)
            {
                if (alpha >= 0)
                {
                    slide_color.a = alpha;
                    alpha -= 0.006f;
                    fade_black.GetComponent<SpriteRenderer>().color = slide_color;
                }
                else
                {
                    fade_black.GetComponent<SpriteRenderer>().color = full_transparent;
                    initial_fade = true;
                    slide_color = new Vector4(1, 1, 1, 1);
                }
            }
            else if (var < 5)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (slides[var].GetComponent<TypeWriter>().finish_typing == true)
                    {
                        slides[var].GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 0);
                        var++;
                    }
                    else
                    {
                        slides[var].GetComponent<TypeWriter>().InstantType();
                    }
                }

                if (slides[var].GetComponent<TypeWriter>().finish_typing == false && slides[var].GetComponent<TypeWriter>().start_typing == false)
                {
                    if (alpha < 1)
                    {
                        alpha += 0.01f;
                        slide_color.a = alpha;
                        if (var != 0)
                        {
                            slides[var].GetComponent<SpriteRenderer>().color = slide_color;
                        }
                    }
                    else
                    {
                        slides[var].GetComponent<TypeWriter>().start_typing = true;
                        slides[var].GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);

                        alpha = 1;
                    }

                }
                else if (slides[var].GetComponent<TypeWriter>().finish_typing == true)
                {
                    if (wait_time < 1)
                    {
                        wait_time += Time.deltaTime * 10f;
                    }
                    else
                    {
                        if (alpha > 0)
                        {
                            alpha -= 0.01f;
                            slide_color.a = alpha;
                            slides[var].GetComponent<SpriteRenderer>().color = slide_color;
                        }
                        else
                        {
                            slides[var].GetComponent<SpriteRenderer>().color = full_transparent;

                            if (transition_time < 1)
                            {
                                transition_time += 20f * Time.deltaTime;
                            }
                            else
                            {
                                // var++;
                                wait_time = 0;
                                transition_time = 0;
                            }
                        }
                    }
                }
            }
            else
            {
                LoadWithScreen("Workshop");
                in_load = true;
            }
        }

        if (set_load)
        {
            if (loading.loaded_in)
            {
                if (!currently_loading)
                {
                    StartCoroutine(LoadAsync(load_location));

                    currently_loading = true;
                }
            }
        }
    }

    public void LoadWithScreen(string load_destination)
    {
        loading_screen.SetActive(true);
        loading.fade_in = true;

        set_load = true;
        load_location = load_destination;
    }

    IEnumerator LoadAsync(string load_destination)
    {
        bool load_completion = false;

        //if (loading.loaded_in)

        AsyncOperation load_op = SceneManager.LoadSceneAsync(load_destination);

        while (!load_op.isDone)
        {
            if (load_op.progress >= 0.89f)
            {
                if (!load_completion)
                {
                    Debug.Log("load Completed");

                    loading.fade_in = false;
                    loading.load_complete = true;

                    DontDestroyOnLoad(loading_screen);

                    load_completion = true;
                }
            }

            yield return null;
        }

    }
}
