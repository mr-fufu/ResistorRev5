using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{


    public GameObject scene_manager;
    public GameObject welder_body;
    public GameObject welder_arm;

    public GameObject dark_welder_body;
    public GameObject dark_welder_arm;

    public GameObject start_game_button;
    public GameObject credits_button;
    public GameObject exit_game_button;

    public GameObject credits_object;
    public Color original_color;
    public Color hightlight_color;
    public GameObject fade_object;

    private bool button_mouse_over;
    private bool credits_showing;

    private int weld_timer;
    public Color fade_color;
    private float alpha_value;
    private SpriteRenderer sprite_render;

    private bool loading;

    // Start is called before the first frame update
    void Start()
    {
        original_color = start_game_button.GetComponentInChildren<UnityEngine.UI.Text>().color;
        weld_timer = 10;
    }

    // Update is called once per frame
    void Update()
    {
        dark_welder_arm.transform.position = welder_arm.transform.position;
        dark_welder_body.transform.position = welder_body.transform.position;

        var mouse_position = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D ray_hit = (Physics2D.Raycast(mouse_position, Vector2.zero, Mathf.Infinity));

        if (ray_hit.collider != null)
        {
            if (ray_hit.collider.gameObject.tag == "MenuClickable")
            {
                button_mouse_over = true;

                ray_hit.collider.gameObject.GetComponentInChildren<UnityEngine.UI.Text>().color = hightlight_color;

                welder_arm.SetActive(true);
                welder_arm.gameObject.transform.position = new Vector2(welder_arm.gameObject.transform.position.x, ray_hit.collider.gameObject.transform.position.y - 100);
                welder_body.SetActive(true);

                dark_welder_arm.SetActive(false);
                dark_welder_body.SetActive(false);

                if (weld_timer <= 0)
                {
                    weld_timer = 10;
                    welder_body.GetComponent<Animator>().SetBool("Welding", true);
                    welder_arm.GetComponent<Animator>().SetBool("Welding", true);
                    welder_arm.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("Welding", true);

                }
                else
                {
                    weld_timer -= 1;
                }
            }
            else
            {
                start_game_button.GetComponentInChildren<UnityEngine.UI.Text>().color = original_color;
                credits_button.GetComponentInChildren<UnityEngine.UI.Text>().color = original_color;
                exit_game_button.GetComponentInChildren<UnityEngine.UI.Text>().color = original_color;

                button_mouse_over = false;

                welder_arm.SetActive(false);
                welder_body.SetActive(false);
            }
        }

        else
        {
            start_game_button.GetComponentInChildren<UnityEngine.UI.Text>().color = original_color;
            credits_button.GetComponentInChildren<UnityEngine.UI.Text>().color = original_color;
            exit_game_button.GetComponentInChildren<UnityEngine.UI.Text>().color = original_color;

            button_mouse_over = false;

            welder_arm.SetActive(false);
            welder_body.SetActive(false);

            dark_welder_arm.SetActive(true);
            dark_welder_body.SetActive(true);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (credits_showing)
            {
                credits_object.SetActive(false);
                credits_showing = false;
            }
            else
            {
                if (button_mouse_over == true)
                {
                    if (ray_hit.collider.gameObject == start_game_button)
                    {
                        scene_manager.GetComponent<SceneManagementScript>().GoToIntro();

                        /*
                        fade_object.SetActive(true);

                        sprite_render = fade_object.GetComponent<SpriteRenderer>();
                        fade_color = fade_object.GetComponent<SpriteRenderer>().color;
                        fade_color.a = 0f;
                        sprite_render.material.color = fade_color;
                        StartCoroutine("fade");
                        */
                    }
                    else if (ray_hit.collider.gameObject == credits_button)
                    {   
                        credits_object.SetActive(true);
                        credits_showing = true;
                    }
                    else if (ray_hit.collider.gameObject == exit_game_button)
                    {
                        Application.Quit();
                    }
                }
            }
        }

        /*
        if (sprite_render != null)
        {
            if (sprite_render.material.color.a >= 0.9)
            {
                fade_color.a = 1f;
                sprite_render.material.color = fade_color;
                StartCoroutine("wait");

                if (!loading)
                {
                    scene_manager.GetComponent<SceneManagementScript>().GoToIntro();
                    loading = true;
                }
            }
        }
        */
    }

    IEnumerator fade()
    {
        for (float fade_alpha = 0f; fade_alpha <= 1; fade_alpha += 0.05f)
        {
            Color fade_color = sprite_render.material.color;
            fade_color.a = fade_alpha;
            sprite_render.material.color = fade_color;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator wait()
    {
        yield return new WaitForSecondsRealtime(3f);
    }
}

