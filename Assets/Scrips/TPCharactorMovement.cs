using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPCharactorMovement : MonoBehaviour
{
    void Start()
    {
        gameObj = this.gameObject;
        chaController = gameObj.GetComponent<CharacterController>();
        Debug.Assert(chaController, "Can not find Component of type \"CharacterController\" in this GameObject");
        currentPosition = gameObj.transform.position;

        objTransform = gameObj.transform;
        tpCamera = GameObject.Find("TPCamera");
        Debug.Assert(tpCamera, "Can not find GameOjbect named \"TPCamera\"");

        charactorMesh = GameObject.Find("BasicMotionsDummy");
        Debug.Assert(charactorMesh, "Can not find GameOjbect named \"BasicMotionsDummy\"");
    }
    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdateJump();
        UpdateRotation();
        //Debug.Log(chaController.isGrounded);
    }

    void FixedUpdate()
    {
        UpdateGravity();
    }


    void UpdateMovement()
    {
        if (chaController && tpCamera)
        {
            //Player Game Object移动
            Vector2 movementKeyState = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            float deltaTime = Time.deltaTime;
            Vector3 rightVec = tpCamera.transform.right;
            rightVec.y = 0;
            rightVec = Vector3.Normalize(rightVec);

            Vector3 forwardVec = tpCamera.transform.forward;
            forwardVec.y = 0;
            forwardVec = Vector3.Normalize(forwardVec);

            Vector3 movementValue = movementKeyState.x * rightVec + movementKeyState.y * forwardVec;
            chaController.Move(movementValue * movementSpeed * deltaTime);

            //up down movement
            float moveUpSpeed = 10f;
            if (Input.GetKey(KeyCode.E))
                chaController.Move(new Vector3(0, moveUpSpeed * Time.deltaTime, 0));
            if (Input.GetKey(KeyCode.Q))
                chaController.Move(new Vector3(0, -moveUpSpeed * Time.deltaTime, 0));
        }
    }

    private void UpdateJump()
    {
    }

    private void UpdateRotation()
    {
        Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (inputDirection.magnitude > Mathf.Epsilon)
        {
            Matrix4x4 localToWorld = tpCamera.transform.localToWorldMatrix;
            curInputDirection = localToWorld * inputDirection;
            curInputDirection.y = 0;
        }

        Vector3 curMeshDir = charactorMesh.transform.forward;
        curMeshDir.y = 0;
        float deltaAngle = Vector3.Angle(curMeshDir, curInputDirection);
        Vector3 crossVec = Vector3.Cross(curInputDirection, curMeshDir);
        if (crossVec.y > 0)
            deltaAngle = -deltaAngle;
        charactorMesh.transform.Rotate(new Vector3(0, deltaAngle * Time.deltaTime * rotationLerpSpeed, 0));
    }

    private void UpdateGravity()
    {
        float deltaYPos = gameObj.transform.position.y - currentPosition.y;
        currentPosition = gameObj.transform.position;
       
        currentYSpeed = deltaYPos / Time.deltaTime;
        float gravityAccel = -9.8f;
        float nextYSpeed = currentYSpeed + gravityAccel * Time.deltaTime;
        chaController.Move(new Vector3(0, nextYSpeed * Time.deltaTime, 0));
        Debug.Log(nextYSpeed);
    }

    private CharacterController chaController;
    private GameObject gameObj;
    private Transform objTransform;
    private GameObject tpCamera;
    private GameObject charactorMesh;
    private Vector3 curInputDirection;
    private float rotationLerpSpeed = 30f;
    private Vector3 currentPosition;
    private float currentYSpeed = 0f;

    public float movementSpeed = 15f;

}
