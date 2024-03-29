﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameController controller;
    Animator myAnimator;



    //Is the player jumping from one edge to the other?

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("Game Controller").GetComponent<GameController>();
        myAnimator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {//brute force for the snake facing animations
         /*if (Controller.playerIsMoving == false)
        {
           if (Input.GetAxisRaw("Vertical") > 0)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            if (Input.GetAxisRaw("Vertical") < 0)
            {
            transform.localRotation = Quaternion.Euler(0, 0, -90);
            }
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
            transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
           }
        } */

        if (controller.snakeScore > 0)//sets snake animation based on snakeScore Value
        {
            myAnimator.SetBool("SnakeHasFood", true);
        }
        else
        {
            myAnimator.SetBool("SnakeHasFood", false);
        }
        

    }


}
