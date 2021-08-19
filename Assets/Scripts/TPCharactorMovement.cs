using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPCharactorMovement : MonoBehaviour
{
    void Start()
    {
        tpCharactor = this.gameObject;
        moveController = tpCharactor.GetComponent<CharacterController>();
        Debug.Assert(moveController, "Can not find Component of type \"CharacterController\" in this GameObject");
        currentPosition = tpCharactor.transform.position;

        tpCameraPivot = GameObject.Find("TPCameraPivot");
        Debug.Assert(tpCameraPivot, "Can not find GameOjbect named \"TPCameraPivot\"");

        charactorMesh = GameObject.Find("BasicMotionsDummy");
        Debug.Assert(charactorMesh, "Can not find GameOjbect named \"BasicMotionsDummy\"");
    }
    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdateRotation();
        //Debug.Log(chaController.isGrounded);
    }

    void FixedUpdate()
    {
        UpdateGravity();
    }


    void UpdateMovement()
    {
        if (moveController && tpCameraPivot)
        {
            //Player Game Object移动
            Vector2 movementKeyState = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            float deltaTime = Time.deltaTime;
            Vector3 rightVec = tpCameraPivot.transform.right;
            rightVec.y = 0;
            rightVec = Vector3.Normalize(rightVec);

            Vector3 forwardVec = tpCameraPivot.transform.forward;
            forwardVec.y = 0;
            forwardVec = Vector3.Normalize(forwardVec);

            Vector3 movementValue = movementKeyState.x * rightVec + movementKeyState.y * forwardVec;
            moveController.Move(movementValue * movementSpeed * deltaTime);

            // //up down movement
            // float moveUpSpeed = 10f;
            // if (Input.GetKey(KeyCode.E))
            //     chaController.Move(new Vector3(0, moveUpSpeed * Time.deltaTime, 0));
            // if (Input.GetKey(KeyCode.Q))
            //     chaController.Move(new Vector3(0, -moveUpSpeed * Time.deltaTime, 0));
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
            Matrix4x4 localToWorld = tpCameraPivot.transform.localToWorldMatrix;
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
        float deltaYPos = tpCharactor.transform.position.y - currentPosition.y;
        currentPosition = tpCharactor.transform.position;
       
        currentYSpeed = deltaYPos / Time.deltaTime;
        float gravityAccel = -9.8f;
        float nextYSpeed = currentYSpeed + gravityAccel * Time.deltaTime;
        moveController.Move(new Vector3(0, nextYSpeed * Time.deltaTime, 0));
    }

    private CharacterController moveController;
    private GameObject tpCharactor;
    private GameObject tpCameraPivot;
    private GameObject charactorMesh;
    private Vector3 curInputDirection;
    private float rotationLerpSpeed = 30f;
    private Vector3 currentPosition;
    private float currentYSpeed = 0f;

    public float movementSpeed = 15f;

}
