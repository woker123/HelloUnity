﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPCameraRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        tpCameraPivot = this.gameObject;
        tpCamera = GameObject.Find("TPCamera");
        cameraArmLength = (tpCameraPivot.transform.position - tpCamera.transform.position).magnitude;
        
        Debug.Assert(tpCamera, "Can not find GameObject named \"TPCameraPivot\"");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRotation();
        UpdateCollisionBias();
    }

    void TurnRight(float angle)
    {
        tpCameraPivot.transform.Rotate(0, angle, 0, Space.World);   
    }

    void TurnUp(float angle)
    {
        float upLimit = 300;
        float downLimit = 90;
        float currentAngle = tpCameraPivot.transform.eulerAngles.x;
        
        float nextAngle = currentAngle + angle;
        if(nextAngle < downLimit || nextAngle > upLimit)
            tpCameraPivot.transform.Rotate(angle, 0, 0, Space.Self);
    }

    void UpdateRotation()
    {
        if(Input.GetKey(KeyCode.Mouse1))
        {
            if(tpCameraPivot)
            {
                mouseSpeed = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                TurnUp(-mouseSpeed.y * turnSpeed * Time.deltaTime);
                TurnRight(mouseSpeed.x * turnSpeed * Time.deltaTime);
            }
        }
    }

    void UpdateCollisionBias()
    {
        if(tpCameraPivot && tpCamera)
        {
            Ray ray = new Ray(tpCameraPivot.transform.position, tpCamera.transform.position - tpCameraPivot.transform.position);
            RaycastHit hitResult;
            bool isCollided = Physics.Raycast(ray, out hitResult, Mathf.Infinity);
            if(isCollided && hitResult.collider.name != "TPCharactor")
            {
                float angle = Vector3.Angle(hitResult.normal, -ray.direction);
                float verticalBias = 0.1f;
                float directBias = 0f;
                if(Mathf.Approximately(angle, 90f))
                    directBias = verticalBias;
                else
                    directBias = verticalBias / Mathf.Cos(Mathf.Deg2Rad * angle);

                float hitDistance = hitResult.distance < cameraArmLength ? (hitResult.distance - directBias) : cameraArmLength;
                tpCamera.transform.position = tpCameraPivot.transform.position + ray.direction * hitDistance;
            }
        }
    }

    private GameObject tpCameraPivot;
    private GameObject tpCamera;
    private Vector2 mouseSpeed;
    private float cameraArmLength = 0f;
    public float turnSpeed = 1500;

}
