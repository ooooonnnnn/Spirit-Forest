using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] private Animator animator;
    
    private int movementState = 0; //to control the animation

    private int startingPosition = 0; // if the starting position is 0, means the character is in the middle
    [SerializeField] float distanceOfSwipe = 2f;
    private float locationOfPlayerXaxis;
    private float locationOfPlayerYaxis;
    private float locationOfPlayerZaxis;
    public Vector3 jump;
    public float jumpForce = 2f;
    public bool isGrounded;
    Rigidbody rb;


    private void Start()
    {
     rb = GetComponent<Rigidbody>();
        startingPosition = 0;
        jump = new Vector3(0, 2, 0);
    }
    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
    // Update is called once per frame
    void Update()
    {
       // Debug.Log(startingPosition);
        locationOfPlayerXaxis = transform.position.x;
        locationOfPlayerYaxis = transform.position.y;
        locationOfPlayerZaxis = transform.position.z;

        transform.Translate(0, 0, speed * Time.deltaTime); // makes the player move forward


        if (Input.GetKeyDown(KeyCode.D))
        {
            if (startingPosition == 0 || startingPosition == -1)
            {
                transform.position = new Vector3((float)locationOfPlayerXaxis + distanceOfSwipe, locationOfPlayerYaxis, locationOfPlayerZaxis); // go right

            }
            if (startingPosition == 0)
            {
                startingPosition++;
            }
            else if (startingPosition == -1)
            {
                startingPosition++;
            }
            

        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (startingPosition == 0 || startingPosition == 1)
            {
                transform.position = new Vector3((float)(locationOfPlayerXaxis - distanceOfSwipe), locationOfPlayerYaxis, locationOfPlayerZaxis); // go left

            }
            if (startingPosition == 0)
            {
                startingPosition--;// -1 position means the character is on the left lane
            }
            else if (startingPosition == 1)
            {
                startingPosition--;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Space was pressed");
            isGrounded = false;
            rb.AddForce(jump * jumpForce, ForceMode.Impulse);

            // Debug.Log(isGrounded);

        }
        else
        {
            //isGrounded = true;
        }
        // Debug.Log(isGrounded);


        //animation
        movementState = 2;


        animator.SetInteger("movementState", movementState);
    }



}


