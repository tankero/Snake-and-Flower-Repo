using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPickup : MonoBehaviour
{
    [SerializeField] AudioClip foodPickupSFX;
    private bool addedToScore;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Food being eaten by: " + collision.gameObject);
        if (!addedToScore)
        {
            addedToScore = true;
            FindObjectOfType<GameController>().OnSnakeCollisionWithFood();
            //AudioSource.PlayClipAtPoint(foodPickupSFX, Camera.main.transform.position);//future implementation of an audio effect
            Destroy(gameObject);
        }
    }
}
