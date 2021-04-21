using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public float turnSpeed = 4.0f;
    public float moveSpeed = 2.0f;
    public float minTurnAngle = -180.0f;
    public float maxTurnAngle = 180.0f;
    public GameObject objtofollow;

    private float rotX;
    private Transform followedObjTf;
    private Rigidbody followedObjRb;
    void Start()
    {
        followedObjTf = objtofollow.GetComponent<Transform>();
        followedObjRb = objtofollow.GetComponent<Rigidbody>();
    }

    void Update()
    {
        MouseAiming();
    }

    void LateUpdate()
    {
        KeyboardMovement();
    }

    void MouseAiming()
    {
        // get the mouse inputs
        float y = Input.GetAxis("Mouse X") * turnSpeed;
        rotX += Input.GetAxis("Mouse Y") * turnSpeed;

        // clamp the vertical rotation
        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);

        // rotate the camera
        transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0);
    }

    void KeyboardMovement()
    {
        Vector3 dir = new Vector3(0, 0, 1);

        //dir.x = Input.GetAxis("Horizontal");
        //dir.z = Input.GetAxis("Vertical");

        transform.position = new Vector3(followedObjTf.position.x, followedObjTf.position.y, transform.position.z + Input.GetAxis("Mouse ScrollWheel") * moveSpeed * Time.deltaTime);

    }
}
