using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPickup : MonoBehaviour
{
    [SerializeField] AudioClip foodPickupSFX;
    [SerializeField] int pointsForfoodPickup = 100;
    private bool addedToScore;

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!addedToScore)
        {
            addedToScore = true;
            FindObjectOfType<GameController>().AddToScore(pointsForfoodPickup);
            //AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);//future implementation of an audio effect
            Destroy(gameObject);
        }
    }
}
