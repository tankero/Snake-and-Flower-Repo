using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDropOff : MonoBehaviour
{
    [SerializeField] AudioClip foodDropSFX;
    private GameController controller;

    //Adding start to find reference to game controller.
    void Start()
    {
        controller = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Food being dropped off by: {collision.gameObject}");
        controller.OnSnakeCollisionWithFlower();
    }

}

