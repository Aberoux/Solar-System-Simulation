using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeScale : MonoBehaviour
{
    public float ScaletoEarth;
    public GameObject objtoscaleto;

    private Transform selfTfcomp;
    private Constants constants;
    private Transform objTf;
    private Vector3 dist;
    private float max;
    private float min;
    private float constant;
    private GameObject[] otherBodies;
    
    // Start is called before the first frame update
    void Start()
    {
        // initialise important variables
        selfTfcomp = GetComponent<Transform>();
        constants = GameObject.Find("ConstantsObj").GetComponent<Constants>();
        objTf = objtoscaleto.GetComponent<Transform>();
        max = ScaletoEarth*constants.MaxScale;
        min = ScaletoEarth*constants.MinScale;
        constant = constants.MinDistance - ((max/(max-min))/constants.scale);
        if (gameObject.name == "ROCKET") 
        {
            otherBodies = GameObject.FindGameObjectsWithTag("MassiveBody");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // vary the size of each planet depending on their distance from the rocket
        dist = selfTfcomp.position - objTf.position;

        // the rocket needs to scale to the closest planet
        if (gameObject.name == "ROCKET")
        {
            foreach (GameObject i in otherBodies) 
            {
                // that is not itself
                if (i != gameObject) 
                {
                    Vector3 tempdist = selfTfcomp.position - i.GetComponent<Transform>().position;
                    if (tempdist.magnitude < dist.magnitude) 
                    {
                        dist = tempdist;
                    }
                } 
            }
        }
        // if closer than minimum distance, stay at constant size
        if (dist.magnitude < constants.MinDistance)
            {
                selfTfcomp.localScale = min * new Vector3(1,1,1);
            }
        else 
        {
            // else follow a function tends to the maximum size as the rocket gets further and further away
            selfTfcomp.localScale = max * (1-(1/(constants.scale*(dist.magnitude-constant)))) * new Vector3(1,1,1);
        }
    }
}
