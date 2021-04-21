using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Constants : MonoBehaviour
{
    public float biG;
    public float SpeedScale;
    public float MassScale;
    public float DistanceScale;
    // for the calculation of g
    private float Umass;
    private float Utime;
    private float Udistance;
    private float SImass;
    private float SItime;
    private float SIdistance;
    private Text UISpeedText;

    // variables responsible to the dynamic sizing of the planets
    public float MaxScale;
    public float MinScale;
    public float scale;
    public float MinDistance;

    void Start()
    {
        // define what each unity unit corresponds to in SI at the start of simulation.
        Umass = 1.0f;
        SImass = 5.9722E+24f; // kg
        Udistance = 50.0f;
        SIdistance = 1.4960E+11f; // m
        Utime = 60.0f; 
        SItime = 3.154E+7f; // s

        // at the start, the scales are 1.
        SpeedScale = 1.0f;
        MassScale = 1.0f;
        DistanceScale = 1.0f;

        // get g
        biG = 6.6741E-11f * (Mathf.Pow((Udistance*DistanceScale)/SIdistance,3.0f) / (((Umass*MassScale)/SImass) * Mathf.Pow(Utime/(SItime*SpeedScale),2.0f)) );
    
        UISpeedText = GameObject.Find("SpeedScale").GetComponent<Text>();

    }

    //better control detection
    void Update() 
    {
        if (Input.GetKeyDown("1"))
        {
            SpeedScale = SpeedScale / 2;
        }
        if (Input.GetKeyDown("2"))
        {
            SpeedScale = SpeedScale * 2;
        }

        // update g
        biG = 6.6741E-11f * (Mathf.Pow((Udistance*DistanceScale)/SIdistance,3.0f) / (((Umass*MassScale)/SImass) * Mathf.Pow(Utime/(SItime*SpeedScale),2.0f)) );
        // update display
        UISpeedText.text = "Speed: x" + SpeedScale;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        }
    }

    
}
