using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlocker : MonoBehaviour
{
    public bool spawnBlocked;

    private Collider2D[] check_colliders;
    private int scan_size = 20;
    private ContactFilter2D collider_filter = new ContactFilter2D();
    private int empty_count;

    public GameObject spawnLight;
    public GameObject spawnShadow;
    public GameObject networkColor;

    private bool spawnLighting;
    private Vector4 originalColor;
    private float fade;
    private float intensity;
    private bool originalColorSet;

    // Start is called before the first frame update
    void Start()
    {
        spawnBlocked = false;
        spawnLighting = false;
        collider_filter = collider_filter.NoFilter();
        spawnLight.GetComponent<Light>().intensity = 0;
    }

    void Update()
    {
        if (!originalColorSet)
        {
            if (networkColor.GetComponent<NetworkColor>().colored)
            {
                originalColor = spawnShadow.GetComponent<SpriteRenderer>().color;
                spawnShadow.GetComponent<SpriteRenderer>().color = new Vector4(originalColor.x, originalColor.y, originalColor.z, 0);
                originalColorSet = true;
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
                    spawnBlocked = true;
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
            spawnBlocked = false;
        }

        if (spawnBlocked)
        {
            if (!spawnLighting)
            {
                spawnLighting = true;

                spawnLight.SetActive(true);
                spawnLight.GetComponent<Light>().intensity = 12f;

                spawnShadow.SetActive(true);
                spawnShadow.GetComponent<SpriteRenderer>().color = new Vector4(originalColor.x, originalColor.y, originalColor.z, 1);

                fade = 1;
                intensity = 12;
            }
        }

        if (spawnLighting && !spawnBlocked)
        {
            fade -= 0.01f;
            intensity -= 0.12f;

            if (fade <= 0)
            {
                spawnLighting = false;

                spawnLight.GetComponent<Light>().intensity = 0;
                spawnShadow.GetComponent<SpriteRenderer>().color = new Vector4(originalColor.x, originalColor.y, originalColor.z, 0);
            }
            else
            {
                spawnLight.GetComponent<Light>().intensity = intensity;
                spawnShadow.GetComponent<SpriteRenderer>().color = new Vector4(originalColor.x, originalColor.y, originalColor.z, fade);
            }


        }

    }
}
