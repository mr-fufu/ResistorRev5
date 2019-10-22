using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerScript : MonoBehaviour
{
    public bool flicker;
    public GameObject flicker_light;
    public GameObject sparks;
    public Vector2 spark_position;

    private Color active_color = new Color(255, 255, 255, 1);
    private Color inactive_color = new Color(255, 255, 255, 0);

    private int flicker_count;
    public bool self_flicker;

    public bool detect_flicker;
    private bool holdover;

    // Start is called before the first frame update
    void Start()
    {
        if (self_flicker)
        {
            flicker_light = gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (detect_flicker)
        {
            if (!holdover)
            {
                flicker = false;
            }
            else
            {
                holdover = false;
            }
        }

        if (flicker)
        {
            //if (scene_manager.GetComponent<SceneManagementScript>().start_true)
            //{
                flicker_count++;

                if (flicker_count < 8 || flicker_count > 12 && flicker_count < 16)
                {
                    if (flicker_count == 4 || flicker_count == 14)
                    {
                        spark_position = new Vector2(flicker_light.transform.position.x + Random.Range(-5, 6), flicker_light.transform.position.y + Random.Range(-15, 16));
                        var gen_spark = Instantiate(sparks, spark_position, transform.rotation);
                        gen_spark.transform.localScale = new Vector2(2, 2);
                    }

                    flicker_light.gameObject.GetComponent<SpriteRenderer>().color = inactive_color;

                }
                else
                {
                flicker_light.gameObject.GetComponent<SpriteRenderer>().color = active_color;
            }

                if (flicker_count >= 50)
                {
                    flicker_count = 0;
                    flicker = false;
                }
            //}
        }
    }

    private void OnMouseOver()
    {
        if (detect_flicker)
        {
            flicker = true;

            holdover = true;
        }
    }
}
