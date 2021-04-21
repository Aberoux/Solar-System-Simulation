using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RocketControls : MonoBehaviour
{
    public Vector3 ForwardDir;
    public float TurnSpeed;

    private Constants constants;
    private Transform selfTfcomp;
    private Rigidbody selfRbcomp;
    private MassiveBody SunMb;
    private Transform EarthTf;
    private Rigidbody EarthRb;
    private GameObject CurrentDestination;
    private List<GameObject> Destinations;

    private Text UIDistanceText;
    private Text UIStageText;
    private Text UIDestinationText;
    private Text UITimeInTransitText;

    float[] HohmanNum;
    private float startR;
    private float endR;
    private int stage;
    private int count;

    private bool SpeedUp;
    private bool SlowDown;
    private bool ChangeDestinations;
    private bool Applydv;
    
    // Start is called before the first frame update
    void Start()
    {
        selfTfcomp = GetComponent<Transform>();
        selfRbcomp = GetComponent<Rigidbody>();
        constants = GameObject.Find("ConstantsObj").GetComponent<Constants>();
        SunMb = GameObject.Find("Sun").GetComponent<MassiveBody>();
        EarthTf = GameObject.Find("Earth").GetComponent<Transform>();
        EarthRb = GameObject.Find("Earth").GetComponent<Rigidbody>();

        UIDistanceText = GameObject.Find("Distance").GetComponent<Text>();
        UIStageText = GameObject.Find("Stage").GetComponent<Text>();
        UIDestinationText = GameObject.Find("Destination").GetComponent<Text>();
        UITimeInTransitText = GameObject.Find("TimeInTransit").GetComponent<Text>();

        CurrentDestination = GameObject.Find("Mars");
        UIDestinationText.text = "Destination: " + CurrentDestination.name;
        UITimeInTransitText.text = "Time in Transit: 41.51s";
        GameObject[] Objects = GameObject.FindGameObjectsWithTag("MassiveBody");
        Destinations = new List<GameObject>();
        
        // include only the other planets as destinations
        foreach (GameObject Object in Objects)
        {
            if (!(Object.name == "Sun") & !(Object.name == "Earth") & !(Object.name == "ROCKET"))
            {
                Destinations.Add(Object);
            }
        }

        stage = 0;
        count = 0;
        startR = 0.0f;
    }

    // better control detection
    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Tab) & stage == 0)
        {
            int index = Destinations.FindIndex(i => i.name == CurrentDestination.name);
            CurrentDestination = Destinations[(index + 1) % Destinations.Count];
            UIDestinationText.text = "Destination: " + CurrentDestination.name;
            ChangeDestinations = true;
        }

        if (Input.GetKeyDown("1"))
        {
            SlowDown = true;
        }
        if (Input.GetKeyDown("2"))
        {
            SpeedUp = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Applydv = true;
        }
    }

    void FixedUpdate()
    {
        // this is just to stop double inputs
        if (stage == 1)
            {
                count += 1;
            }
        
        if (Applydv | ChangeDestinations | SpeedUp | SlowDown)
        {
            //calculate hohmann delta vs
            endR = CurrentDestination.GetComponent<Transform>().position.magnitude;
            if (startR == 0.0f)
            {
                HohmanNum = Hohman(selfTfcomp.position.magnitude,endR);
            } else 
            {
                HohmanNum = Hohman(startR,endR);
            }
            
            
            if (Applydv)
            {
                // set direction tangent to orbit (wrt sun)
                ForwardDir = Vector3.Cross(new Vector3(0.0f,0.0f,1.0f),selfTfcomp.position.normalized).normalized; 
                // get component of velocity orbit (wrt earth)
                Vector3 EarthOrbitVel = selfRbcomp.velocity-EarthRb.velocity;

                // first stage: dv to get to elliptical orbit
                if (stage == 0) 
                {
                    startR = selfTfcomp.position.magnitude;
                    HohmanNum = Hohman(startR,endR);
                    // apply first stage dv
                    selfRbcomp.velocity = selfRbcomp.velocity + ((HohmanNum[0]) * ForwardDir) - (EarthOrbitVel/(2.0f));  
                    stage = 1;
                }

                // second stage: dv to stay in the other orbit
                if (stage == 1 & count > 100) 
                {
                    selfRbcomp.velocity = selfRbcomp.velocity + (HohmanNum[1]  * ForwardDir); 
                    stage = 2;
                }
                Applydv = false;
            }
            
            if (ChangeDestinations | SpeedUp | SlowDown)
            {
                UITimeInTransitText.text = "Time in Transit: " + HohmanNum[2].ToString("F2") + "s";
                ChangeDestinations = false;
                SpeedUp = false;
                SlowDown = false;
            }
            
        }

        // keep track of distance
        UIDistanceText.text = "Distance from Sun: " + selfTfcomp.position.magnitude.ToString("F2"); 
        UIStageText.text = "Stage: " + stage; 

        // turn the rocket in velocity direction
        Vector3 ahead = new Vector3(0.0f,0.0f,0.0f);
        if (stage == 0)
        {
            // while orbiting the earth, turn towards orbit velocty
            Vector3 relvel = selfRbcomp.velocity-EarthRb.velocity;
            ahead = selfTfcomp.position + (relvel * 10);
        } else
        {
            ahead = selfTfcomp.position + (selfRbcomp.velocity * 10);
        }
        
        selfTfcomp.LookAt(ahead);
        selfTfcomp.Rotate(Vector3.right * 90);
    }

    // to calculate hohmann transfer values
    float[] Hohman(float r1, float r2)
    {
        float deltav1 = Mathf.Sqrt((constants.biG*SunMb.Mass)/r1) * (Mathf.Sqrt((2*r2)/(r1+r2))-1);
        float deltav2 = Mathf.Sqrt((constants.biG*SunMb.Mass)/r2) * (1-Mathf.Sqrt((2*r1)/(r1+r2)));
        float transitime = Mathf.PI * Mathf.Sqrt(Mathf.Pow(r1+r2,3)/(8*constants.biG*SunMb.Mass));

        float[] info = {deltav1,deltav2,transitime};
        return info;
    }

}
