using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDropOff : MonoBehaviour
{
    [SerializeField] AudioClip foodDropSFX;
    public int flowerTotal = 0;
    [SerializeField] Sprite wiltStage1;
    [SerializeField] Sprite wiltStage2;
    [SerializeField] Sprite wiltStage3;
    [SerializeField] Sprite growStage1;
    [SerializeField] Sprite growStage2;
    [SerializeField] Sprite growStage3;
    [SerializeField] int growValue2 = 500;
    [SerializeField] int growValue3 = 1000;
    private GameController controller;
    [SerializeField] private int foodTimeValue = 5;
    private SpriteRenderer myRenderer;
    private Sprite isWilting;
    Animator myAnimator;

    //Adding start to find reference to game Controller.
    void Start()
    {
        controller = GameObject.Find("Game Controller").GetComponent<GameController>();
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        isWilting = wiltStage1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        flowerTotal += controller.snakeScore; //add to flower score
        controller.snakeScore = 0; //set snake food to 0
        Debug.Log("Total Flower points:" + flowerTotal);
        controller.AddSecondsToFlower(foodTimeValue);
        Debug.Log($"Food being dropped off by: {collision.gameObject}");
        controller.OnSnakeCollisionWithFlower();
    }

    public void Update()
    {
        
        if (flowerTotal >= growValue2)
        {
            myAnimator.enabled = true;
            myAnimator.SetTrigger("Grow2");
            isWilting = wiltStage2;
        }
        if (flowerTotal >= growValue3)
        {
            myAnimator.enabled = true;
            myAnimator.SetTrigger("Grow3");
            isWilting = wiltStage3;
        }

        if (controller.flower.SecondsRemaining <= controller.flower.WiltThreshold)
         {
             myAnimator.enabled = false;
             myRenderer.sprite = isWilting;
             //Debug.Log("WILTING CHANGE");
         }

        if (controller.flower.SecondsRemaining > controller.flower.WiltThreshold)
        {
            myAnimator.enabled = true;
        }
    }
}

