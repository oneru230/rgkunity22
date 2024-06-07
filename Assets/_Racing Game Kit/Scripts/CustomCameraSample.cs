using System;
using UnityEngine;
using RacingGameKit.Interfaces;


public class CustomCameraSample : MonoBehaviour, IRGKCamera
{
    public Transform target;
    // The distance in the x-z plane to the target
    public float distance = 5;
    // the height we want the camera to be above the target
    public float height = 2;
    // How much we 
    public float heightDamping = 3;
    public float rotationDamping = 3;


    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            // Calculate the current rotation angles
            float wantedRotationAngle = target.eulerAngles.y;
            float wantedHeight = target.position.y + height;

            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

            // Damp the rotation around the y-axis
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

            // Damp the height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            // Convert the angle into a rotation
            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target

            Vector3 pos = target.position;
            pos -= currentRotation * Vector3.forward * distance;
            pos.y = currentHeight;
            transform.position = pos;


            // Always look at the target
            transform.LookAt(target);
        }
    }


    //******************CODES BELOW FOR IRGKCAMERA INTERFACE******************
    public string ControlBindingCameraBack
    {
        set { }
    }

    public string ControlBindingCameraChange
    {
        set { }
    }

    public string ControlBindingCameraLeft
    {
        set { }
    }

    public string ControlBindingCameraRight
    {
        set { }
    }

    public int CurrentCount
    {
        set { }
    }

    public bool IsStartupAnimationEnabled
    {
        set { }
    }

    public System.Collections.Generic.List<Transform> TargetObjects
    {
        get
        {
            return null;
        }
        set
        {

        }
    }

    public Transform TargetVehicle
    {
        set { target = value; }
    }

}