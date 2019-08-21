using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameController controller;


    //Is the player jumping from one edge to the other?
    public bool Traversing = false;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
