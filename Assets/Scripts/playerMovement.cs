using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;

public class playerMovement : MonoBehaviour
{
    //moves two transforms: one that is centered in the trail, and it's child which is the actual player model and collider 
    
    [SerializeField] public float speed;
    [SerializeField] private Animator animator;
    [FormerlySerializedAs("playerModel")] [SerializeField] private Transform playerTransform;
    
    private int movementState = 0; //to control the animation
    private float upVelocity;//current upwards velocity
    [Header("Jump Parameters")]
    [SerializeField] private float gravity;
    [SerializeField] private float groundLevel;
    [SerializeField] private float jumpHeight;
    
    private int lane = 0; // if the starting position is 0, means the character is in the middle
    [Space] 
    [SerializeField] private GenerationManager generationManager;
    private float laneWidth;
    private float positionInterpolant = 0;

    private void Start()
    {
        laneWidth = generationManager.laneWidth;
    }

    void Update()
    {
        generationManager.InerpolateToTransform(positionInterpolant, transform); //updates parent transform (this one's) position
        UpdateLane();
        playerTransform.localPosition = lane * laneWidth * Vector3.right;
        positionInterpolant += speed * Time.deltaTime;
        
        Jump(); //takes player input to jump and moves player model accordingly
       
        //animation
        movementState = 2;
        animator.SetInteger("movementState", movementState);
    }


    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            upVelocity = math.sqrt(2 * gravity * jumpHeight);
        }
        
        float distanceToFall;
        if (IsGrounded())
        {
            distanceToFall = playerTransform.position.y - groundLevel;
            upVelocity = 0;
        }
        else
        {
            distanceToFall = upVelocity * Time.deltaTime;
            upVelocity -= gravity * Time.deltaTime;
        }
        
        playerTransform.Translate(Vector3.up * distanceToFall);
    }

    private bool IsGrounded()
    {
        return transform.position.y >= groundLevel;
    }

    private void UpdateLane()
    {
        //uses player input to change lanes
        if (Input.GetKeyDown(KeyCode.D) && lane <= 0)
        {
            lane++;
        }

        if (Input.GetKeyDown(KeyCode.A) && lane >= 0)
        {
            lane--;
        }
    }
}


