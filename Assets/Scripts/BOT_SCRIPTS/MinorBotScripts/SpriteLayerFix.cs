using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayerFix : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = gameObject.GetComponentInParent<SpriteRenderer>().sortingLayerName;
    }
}
