using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDropOff : MonoBehaviour
{
    [SerializeField] AudioClip foodDropSFX;
    public int flowerTotal = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(GameController.snakeScore + "Food being dropped off by: " + collision.gameObject);
        flowerTotal += GameController.snakeScore; //add to flower score
        GameController.snakeScore = 0; //set snake food to 0
        Debug.Log("Total Flower points:" + flowerTotal);
    }

}

