using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPickup : MonoBehaviour
{
    [SerializeField] AudioClip foodPickupSFX;
    [SerializeField] int pointsForfoodPickup = 100;
    private bool addedToScore;
    private GameController controller;

    private void Start()
    {
        controller = FindObjectOfType<GameController>();
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Food being eaten by: " + collision.gameObject);
        if (!addedToScore)
        {
            var contactPoint = collision.GetContact(0);

            Vector2Int collisionPosition = 
                new Vector2Int(
                    (int)Mathf.Round(contactPoint.point.x), 
                    (int)Mathf.Round(contactPoint.point.y));

            addedToScore = true;
            controller.AddToScore(pointsForfoodPickup);
            controller.OnSnakeCollisionWithFood(collisionPosition);
            //AudioSource.PlayClipAtPoint(foodPickupSFX, Camera.main.transform.position);//future implementation of an audio effect
            Destroy(gameObject);
        }
    }
}
