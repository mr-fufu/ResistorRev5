using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeWriter : MonoBehaviour
{
    public Text typed_text;
    public Text input_text;
    public string input_string;

    public bool start_typing;
    public bool finish_typing;

    public string output_string = null;
    public int string_count = 0;

    private float type_counter;

    public bool use_string;
    public bool typing_paused;
    public bool no_pause;
    public bool mini_pause;

    public char[] character;
    public bool fast_type;
    public bool instant_type;

    // Start is called before the first frame update
    void Start()
    {
        typed_text.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (type_counter < 1f)
        {
            if (fast_type)
            {
                type_counter += 1000f * Time.deltaTime;
            }
            else if (instant_type)
            {
                type_counter = 1f;
            }
            else
            {
                type_counter += 20f * Time.deltaTime;
            }
        }
        else
        {
            type_counter = 0;

            if (start_typing && !finish_typing)
            {
                if (!use_string)
                {
                    typed_text.text = type(input_text.text);
                }
                else
                {
                    typed_text.text = type_string(input_string);
                }
            }
        }

        //if (!start_typing)
        //{
        //    typing_paused = true;
        //}

        //if (start_typing && typing_paused)
        //{
        //    typing_paused = false;
        //    finish_typing = false;
        //}
    }

    private string type(string text_to_type)
    {
        char[] character = text_to_type.ToCharArray();

        if (string_count == (character.Length))
        {
            finish_typing = true;
            start_typing = false;
        }
        else
        {
            output_string = output_string + character[string_count].ToString();

            if (character[string_count].ToString() == ".")
            {
                if (!no_pause)
                {
                    if (!mini_pause)
                    {
                        type_counter = -20;
                    }
                    else
                    {
                        type_counter = -5;
                    }
                }
            }

            string_count++;
        }

        return output_string;
    }

    private string type_string(string text_to_type)
    {
        character = text_to_type.ToCharArray();

        if (string_count == (character.Length))
        {
            finish_typing = true;
            start_typing = false;
        }
        else
        {
            output_string = output_string + character[string_count].ToString();

            if (character[string_count].ToString() == ".")
            {
                if (!no_pause)
                {
                    if (!mini_pause)
                    {
                        type_counter = -20;
                    }
                    else
                    {
                        type_counter = -5;
                    }
                }
            }

            string_count++;
        }

        return output_string;
    }

    public void clear_character()
    {
        character = new char[0];
    }
}
