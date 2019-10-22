using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerUpdater : MonoBehaviour
{
    public bool preserve_color;
    public bool adjust_layer;
    public int layer_adjust;
    public bool parent_layer_update;

    private SpriteRenderer double_parent_sprite;

    // Start is called before the first frame update
    void Start()
    {
        if (!parent_layer_update)
        {
            double_parent_sprite = transform.parent.transform.parent.GetComponent<SpriteRenderer>();
        }
        else if (parent_layer_update)
        {
            double_parent_sprite = transform.parent.GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().sortingLayerName = double_parent_sprite.sortingLayerName;

        if (adjust_layer)
        {
            GetComponent<SpriteRenderer>().sortingOrder = double_parent_sprite.sortingOrder + layer_adjust;
        }

        if (!preserve_color)
        {
            GetComponent<SpriteRenderer>().color = double_parent_sprite.color;
        }
    }
}
