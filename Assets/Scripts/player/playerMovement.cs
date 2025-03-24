using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    //moves two transforms: one that is centered in the trail, and it's child which is the actual player model and collider 
    
    [SerializeField] public float speed;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerTransform;
    
    private int movementState = 0; //to control the animation
    private float upVelocity;//current upwards velocity
    private float currentHeight;
    [Header("Jump Parameters")]
    [SerializeField] private float gravity;
    [SerializeField] private float groundLevel;
    [SerializeField] private float jumpHeight;
    
    private int lane = 0; // if the starting position is 0, means the character is in the middle
    [Space] 
    [SerializeField] private GenerationManager generationManager;
    private float laneWidth;
    private float positionInterpolant = 0;

    [Space] 
    [SerializeField] private AnimationCurve slowDownCurve;

    private void Start()
    {
        laneWidth = generationManager.laneWidth;
        StartCoroutine(SpeedUp());
    }

    void Update()
    {
        
        generationManager.InterpolateToTransform(positionInterpolant, transform); //updates parent transform (this one's) position
        UpdateLane();
        Jump(); //takes player input to jump and moves player model accordingly
        playerTransform.localPosition = lane * laneWidth * Vector3.right + currentHeight * Vector3.up;
        
        positionInterpolant += speed * Time.deltaTime;


        //animation
        movementState = 2;
        animator.SetInteger("movementState", movementState);
    }
    IEnumerator SpeedUp()
    {
        float maxSpeed = 1.2f;
        while (true)
        {
            if(speed < maxSpeed)
            speed += 0.02f;
            yield return new WaitForSeconds(2f);
        }
    }

    public void SlowDownOnHit()
    {
        StartCoroutine(ModifySpeedCoroutine(Time.time));
    }

    IEnumerator ModifySpeedCoroutine(float startTime)
    {
        while (true)
        {
            float timeDiff = Time.time - startTime;
            if (timeDiff > slowDownCurve.keys.Last().time)
            {
                break;
            }
            
            float originalSpeed = speed;
            speed += slowDownCurve.Evaluate(timeDiff);
            yield return new WaitForEndOfFrame();
            speed = originalSpeed;
        }
    }
    
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            upVelocity = math.sqrt(2 * gravity * jumpHeight);
        }
        currentHeight += upVelocity * Time.deltaTime;
        
        float distanceToFall;
        if (IsGrounded())
        {
            distanceToFall = groundLevel - currentHeight;
            upVelocity = 0;
        }
        else
        {
            distanceToFall = upVelocity * Time.deltaTime;
            upVelocity -= gravity * Time.deltaTime;
        }
        
        currentHeight += distanceToFall;
    }

    private bool IsGrounded()
    {
        return currentHeight <= groundLevel;
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


