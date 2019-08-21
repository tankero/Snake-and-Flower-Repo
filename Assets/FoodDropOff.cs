using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDropOff : MonoBehaviour
{
    [SerializeField] AudioClip foodDropSFX;
    public int flowerTotal = 0;
    private GameController controller;
    [SerializeField] private int foodTimeValue = 5;

    //Adding start to find reference to game controller.
    void Start()
    {
        controller = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(controller.snakeScore + "Food being dropped off by: " + collision.gameObject);
        flowerTotal += controller.snakeScore; //add to flower score
        controller.snakeScore = 0; //set snake food to 0
        Debug.Log("Total Flower points:" + flowerTotal);
        controller.AddSecondsToFlower(foodTimeValue);
    }

}

