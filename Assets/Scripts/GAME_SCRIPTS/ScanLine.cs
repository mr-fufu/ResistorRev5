using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanLine : MonoBehaviour
{
    public GameObject scan_line;

    public float upper_limit;
    public float lower_limit;
    private float scan_span;
    public bool flip_direction;

    public int scan_height;
    public int scan_speed;
    private float scan_counter;
    private float datum;

    // Start is called before the first frame update
    void Start()
    {
        if (!flip_direction)
        {
            scan_line.transform.position = new Vector2(scan_line.transform.position.x, lower_limit);
        }
        else
        {
            scan_line.transform.position = new Vector2(scan_line.transform.position.x, upper_limit);
        }

        scan_height = 0;
    }

    // Update is called once per frame
    void Update()
    {
        datum = gameObject.transform.position.y;

        scan_span = upper_limit - lower_limit;

        scan_counter += scan_speed * Time.deltaTime;
        if (scan_counter >= 1)
        {
            scan_counter = 0;
            if (scan_height < scan_span)
            {
                scan_height++;
            }
            else
            {
                scan_height = 0;
            }
        }

        if (!flip_direction)
        {
            scan_line.transform.position = new Vector2(scan_line.transform.position.x, lower_limit + scan_height + datum);
        }
        else
        {
            scan_line.transform.position = new Vector2(scan_line.transform.position.x, upper_limit - scan_height + datum);
        }
    }
}
