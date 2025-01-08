using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] private Animator animator;
    private int movementState = 0; //to control the animation
    
    // Update is called once per frame
    void Update()
    {
        //movement
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0,0,speed*Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, -speed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, rotationSpeed *Time.deltaTime,0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0,-rotationSpeed * Time.deltaTime ,0);
        }
        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKey(KeyCode.W))
            {
                transform.Translate(0, 0, 2 * speed * Time.deltaTime);
            }
        }
        
        //animation
        movementState = 0;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                movementState = 2;
            }
            else
            {
                movementState = 1;
            }
        }
        animator.SetInteger("movementState", movementState);
    }
}
