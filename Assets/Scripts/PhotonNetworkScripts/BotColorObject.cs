﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class BotColorObject : MonoBehaviour
{
    private Vector4[] spawn_color = new Vector4[4];
    private Vector4[] spawn_color_alt = new Vector4[4];

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        spawn_color[0] = new Vector4(1f, 0.59f, 0.3f, 1f);
        spawn_color[1] = new Vector4(0.6f, 0.85f, 0.24f, 1f);
        spawn_color[2] = new Vector4(0.1f, 0.77f, 1f, 1f);
        spawn_color[3] = new Vector4(0.5f, 1f, 1f, 1f);

        spawn_color_alt[0] = new Vector4(1f, 0.2f, 0.69f, 1f);
        spawn_color_alt[1] = new Vector4(1f, 0.85f, 0.1f, 1f);
        spawn_color_alt[2] = new Vector4(1f, 0.35f, 0.35f, 1f);
        spawn_color_alt[3] = new Vector4(0.5f, 0.4f, 0.1f, 1f);
    }

    public void UpdateColors(int colorIndex, bool alt)
    {
        if (!alt)
        {
            gameObject.GetComponent<SpriteRenderer>().color = spawn_color[colorIndex];
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = spawn_color_alt[colorIndex];
        }
    }

    public Vector4 GetColors()
    {
        //Debug.LogError("Got Colors");

        return gameObject.GetComponent<SpriteRenderer>().color;
    }
}
