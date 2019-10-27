using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlocker : MonoBehaviour
{
    public bool spawn_blocked;
    private Collider2D[] check_colliders;
    private int scan_size = 20;
    private ContactFilter2D collider_filter = new ContactFilter2D();
    private int empty_count;
    public GameObject spawn_light;
    public GameObject spawn_shadow;
    private bool spawn_lighting;
    private Vector4 original_color;
    private float fade;
    private float intensity;
    private bool original_color_set;

    // Start is called before the first frame update
    void Start()
    {
        spawn_blocked = false;
        spawn_lighting = false;
        collider_filter = collider_filter.NoFilter();
        spawn_light.GetComponent<Light>().intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!original_color_set)
        {
            if (spawn_shadow.GetComponent<NetworkColor>().colored)
            {
                original_color = spawn_shadow.GetComponent<SpriteRenderer>().color;
                spawn_shadow.GetComponent<SpriteRenderer>().color = new Vector4(original_color.x, original_color.y, original_color.z, 0);
                original_color_set = true;
            }
        }

        check_colliders = new Collider2D[scan_size];

        for (int setVar = 0; setVar < scan_size; setVar++)
        {
            check_colliders[setVar] = null;
        }

        empty_count = 0;

        gameObject.GetComponent<Collider2D>().OverlapCollider(collider_filter, check_colliders);

        // check through all elements of check_colliders
        // to see if any of them is the engaged_target


        for (int checkVar = 0; checkVar < scan_size; checkVar++)
        {
            if (check_colliders[checkVar] != null)
            {
                if (check_colliders[checkVar].gameObject.tag == "BOT_Player" || check_colliders[checkVar].gameObject.tag == "BOT_Enemy")
                {
                    spawn_blocked = true;
                }
                else
                {
                    empty_count++;
                }
            }
            else
            {
                empty_count++;
            }
        }

        if (empty_count == scan_size)
        {
            spawn_blocked = false;
        }

        if (spawn_blocked)
        {
            if (!spawn_lighting)
            {
                spawn_lighting = true;

                spawn_light.SetActive(true);
                spawn_light.GetComponent<Light>().intensity = 12f;

                spawn_shadow.SetActive(true);
                spawn_shadow.GetComponent<SpriteRenderer>().color = new Vector4(original_color.x, original_color.y, original_color.z, 1);

                fade = 1;
                intensity = 12;
            }
        }

        if (spawn_lighting && !spawn_blocked)
        {
            fade -= 0.01f;
            intensity -= 0.12f;

            if (fade <= 0)
            {
                spawn_lighting = false;

                spawn_light.GetComponent<Light>().intensity = 0;
                spawn_shadow.GetComponent<SpriteRenderer>().color = new Vector4(original_color.x, original_color.y, original_color.z, 0);
            }
            else
            {
                spawn_light.GetComponent<Light>().intensity = intensity;
                spawn_shadow.GetComponent<SpriteRenderer>().color = new Vector4(original_color.x, original_color.y, original_color.z, fade);
            }


        }

    }
}
