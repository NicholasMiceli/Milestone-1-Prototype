using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{

    public CharacterController MyController;
    public float GroundSpeed = 3f;
    public float AerialSpeed = 2f;
    public float Gravity = 5f;
    public float JumpSpeed = 10f;
    public Transform CameraTransform;
    bool canJump = false;
    float verticalVelocity;
    Vector3 velocity;
    Vector3 groundedVelocity;
    Vector3 normal;
    bool onWall = false;
    public Transform playerBody;
    public float mouseSensitivity;
    public Rigidbody body;
    public GameObject player;
    public GameObject seat;
    Quaternion playerRotation;

    private void Start()
    {
        player = GameObject.Find("Player");
        seat = GameObject.Find("Seat");
    }

    private void Update()
    {
        body = GetComponent<Rigidbody>();
        Vector3 myVector = Vector3.zero;

        //get input
        Vector3 input = Vector3.zero;
        input.x = Input.GetAxis("Horizontal");
        input.z = Input.GetAxis("Vertical");
        input = Vector3.ClampMagnitude(input, 1f);

        float MouseX = Input.GetAxis("Mouse X");
        float rotAmountX = MouseX * mouseSensitivity;
        Vector3 targetRotBody = playerBody.rotation.eulerAngles;
        targetRotBody.y += rotAmountX;
        playerBody.rotation = Quaternion.Euler(targetRotBody);

        Vector3 move = transform.right * input.x + transform.forward * input.z;
        MyController.Move(move * GroundSpeed * Time.deltaTime);

        if (MyController.isGrounded)
        {
            myVector = input;
            //rotate player with camera
            //Quaternion inputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(CameraTransform.forward, Vector3.up), Vector3.up);
            //myVector = inputRotation * myVector;
            //myVector *= GroundSpeed;
        }
        else
        {
            myVector = groundedVelocity;
            
        }

        myVector = Vector3.ClampMagnitude(myVector, GroundSpeed);
        myVector = myVector * Time.deltaTime;
        




        //sets jump velocity
        verticalVelocity = verticalVelocity - Gravity * Time.deltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            //gets vector for wall jump
            if (onWall)
            {
                Vector3 reflection = Vector3.Reflect(velocity, normal);

                Vector3 projected = Vector3.ProjectOnPlane(reflection, Vector3.up);
                groundedVelocity = projected.normalized * GroundSpeed + normal * AerialSpeed;
            }
            if (canJump)
            {
                verticalVelocity += JumpSpeed;
            }
        }
        myVector.y = verticalVelocity * Time.deltaTime;


        CollisionFlags flags = MyController.Move(myVector);
        velocity = myVector / Time.deltaTime;

        

        //check jump possibility
        if (((flags & CollisionFlags.Below) !=0))
        {
            groundedVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
            canJump = true;
            onWall = false;
            verticalVelocity = -3f;
            if(input.x == 0 && input.z == 0)
            {
                myVector = Vector3.zero;
            }
            
        }
        else if ((flags & CollisionFlags.Sides) != 0)
        {
            canJump = true;
            onWall = true;
            if (input.x == 0 && input.z == 0)
            {
                myVector = Vector3.zero;
            }
        }
        if((MyController.isGrounded == false) && ((flags & CollisionFlags.Sides) == 0))
        {
            canJump = false;
            onWall = false;
        }
        



        //myVector = Vector3.zero;

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        normal = hit.normal;
    }

    public void ExitMech(Vector3 launchDir, float launchForce)
    {
        transform.parent = null;
        //body.AddForce(launchDir * launchForce);
        //body.AddForce(launchDir * launchForce, ForceMode.Impulse);
        //print("Exit Mech");
    }
}
