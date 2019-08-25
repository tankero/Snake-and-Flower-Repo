using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameController controller;
    Animator myAnimator;



    //Is the player jumping from one edge to the other?
    public bool Traversing = false;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("Game Controller").GetComponent<GameController>();
        myAnimator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {//brute force for the snake facing animations
        if (controller.playerIsMoving == false)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
            transform.localRotation = Quaternion.Euler(0, 0, -90);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
            transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        if (controller.snakeScore > 0)//sets snake animation based on snakeScore Value
        {
            myAnimator.SetBool("SnakeHasFood", true);
        }
        else
        {
            myAnimator.SetBool("SnakeHasFood", false);
        }
        

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Traversing)
        {
            return;
        }
        var directionalVector = new Vector3();
        if (collision.gameObject.name.Contains("Exit"))
        {
            if (collision.gameObject.name.Contains("Up") || collision.gameObject.name.Contains("Down"))
            {
                directionalVector.x = 1;
                directionalVector.y = -1;
            }
            else
            {
                directionalVector.x = -1;
                directionalVector.y = 1;
            }
        }
        controller.GoToOppositeEdge(directionalVector);
        Traversing = true;


    }
}
