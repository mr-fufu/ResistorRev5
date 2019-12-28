using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldingSparks : MonoBehaviour
{
    public GameObject sparks;
    public Transform spark_position;
    public bool non_rotate;
    private Vector3 hold_position;

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
        hold_position = spark_position.transform.position;

        if (non_rotate)
        {
            var spark_clone = Instantiate(sparks, spark_position.position, Quaternion.Euler(Vector3.zero));
            spark_clone.transform.position = new Vector3(hold_position.x, hold_position.y, -40);
        }
        else
        {
            var spark_clone = Instantiate(sparks, spark_position.position, spark_position.rotation);
            //spark_clone.transform.parent = gameObject.transform;
        }
    }
}
