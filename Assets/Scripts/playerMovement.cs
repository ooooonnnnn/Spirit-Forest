using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] private Animator animator;
    private int movementState = 0; //to control the animation
    private int startingPosition = 0; // if the starting position is 0, means the character is in the middle
    [SerializeField] float distanceOfSwipe = 2f;
    private float locationOfPlayerXaxis;
    private float locationOfPlayerYaxis;
    private float locationOfPlayerZaxis;


    private void Start()
    {
        startingPosition = 0;
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(startingPosition);
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
            Debug.Log(startingPosition);

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

        //animation
        movementState = 2;


        animator.SetInteger("movementState", movementState);
    }


}

