using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassiveBody : MonoBehaviour
{
    public float Mass;
    public Vector3 Initialvelocity;
    public Material TrailMat;
    public float TrailWidth;


    private Rigidbody selfRbcomp;
    private Transform selfTfcomp;
    private Constants constants;
    private GameObject[] otherBodies;

    private TrailRenderer OrbitPath;

    private Vector3 fdir;
    private float magravforce;
    private Vector3 totforce;

    private Vector3 curvel;
    private float newvel;

    private bool SpeedUp;
    private bool SlowDown;
    // Start is called before the first frame update
    void Start()
    {
        // initialise important components
        selfRbcomp = GetComponent<Rigidbody>();
        selfTfcomp = GetComponent<Transform>();
        selfRbcomp.velocity = Initialvelocity;
        selfRbcomp.mass = Mass;
        constants = GameObject.Find("ConstantsObj").GetComponent<Constants>();
        otherBodies = GameObject.FindGameObjectsWithTag("MassiveBody");

        // Trail stuff, visual, not important
        OrbitPath = GetComponent<TrailRenderer>();
        if (OrbitPath != null)
        {
        OrbitPath.startWidth = TrailWidth;
        OrbitPath.time = (2*Mathf.PI*selfTfcomp.position.magnitude)/Initialvelocity.magnitude;
        //OrbitPath.endWidth = 0;
        //OrbitPath.startColor = Color.grey;
        //OrbitPath.endColor = Color.grey;
        }

        //debug log rounds, value in vector is still float
    }

    //better control detection
    void Update() 
    {
        if (Input.GetKeyDown("1"))
        {
            SlowDown = true;
        }
        if (Input.GetKeyDown("2"))
        {
            SpeedUp = true;
        }
    }

    private void FixedUpdate() 
    {
        // get the total force on an object due to gravity

        totforce = Vector3.zero;
        // for all massive objects 
        foreach (GameObject i in otherBodies) 
        {
            // that is not itself
            if (i != gameObject) 
            {
                // get distance
                fdir = i.GetComponent<Transform>().position - selfTfcomp.position;
                // get magnitude of gravitational force
                magravforce = (constants.biG*i.GetComponent<Rigidbody>().mass*selfRbcomp.mass)/(fdir.magnitude*fdir.magnitude);
                fdir.Normalize();
                totforce += magravforce*fdir;
            } 
        }
        // apply the total calculated force
        selfRbcomp.AddForce(totforce);

        
        // change the velocity as the speed scale changes
        if (SpeedUp | SlowDown)
        {
            // get current velocity
            //curvel = selfRbcomp.velocity;

            // slow down
            if (SlowDown)
            {
                //newvel = curvel.magnitude / 2;
                curvel = selfRbcomp.velocity * 0.5f;
                SlowDown = false;
            }

            // speed up
            if (SpeedUp)
            {
                //newvel = curvel.magnitude * 2;
                curvel = selfRbcomp.velocity * 2.0f;
                SpeedUp = false;
            }

            //update velocity
            //curvel.Normalize();
            selfRbcomp.velocity = curvel;
        }
    }
}
