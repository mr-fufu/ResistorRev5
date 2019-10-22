using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldingSparks : MonoBehaviour
{
    public GameObject sparks;
    public Transform spark_position;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // Update is called once per frame
    public void generate_sparks()
    {
        var spark_clone = Instantiate(sparks, spark_position.position, spark_position.rotation);
        spark_clone.transform.parent = gameObject.transform;
    }
}
