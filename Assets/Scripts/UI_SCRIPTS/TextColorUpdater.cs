using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextColorUpdater : MonoBehaviour
{
    private Color update_color;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        update_color = GetComponent<UnityEngine.UI.Text>().color;
        update_color.a = transform.parent.gameObject.GetComponent<SpriteRenderer>().color.a;

        GetComponent<UnityEngine.UI.Text>().color = update_color;
    }
}
