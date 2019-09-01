using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    public float Speed = 5;
    public Vector3 MovementPattern;
    public GameController Controller;
    public Vector3 Destination;
    public bool IsMoving = false;

    [Range(0.5f, 5f)]
    public float Cadence = 1.75f;

    public float Lifetime = 10;

    public float LastMoveTime;

    // Start is called before the first frame update
    void Start()
    {
        Controller = GameObject.Find("Game Controller").GetComponent<GameController>();
        Controller.EnemyList.Add(gameObject);
        Destination = transform.position;
        LastMoveTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /* These enemies are meant to traverse the map for as long as they're active. The pre-instantiated pool of enemies is set in the GameController.
     The movement pattern should only have 1/0/-1 as values on either axis. Diagonal movement is also valid (1,1 / -1,-1 / etc..)
     */
    public void Initialize(Vector3 startingPosition, Vector2 movementPattern, float movementCadence, float lifetime)
    {
        LastMoveTime = Time.time;
        Cadence = movementCadence;
        transform.position = startingPosition;
        Destination = startingPosition;
        MovementPattern = new Vector3(movementPattern.x, movementPattern.y, 0f);
        Lifetime = lifetime;
        gameObject.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D colliderObject)
    {
        if (colliderObject.gameObject.name == "PlayerSnake")
        {
            Debug.Log("Stun the snake");
            Controller.StunSnake();
        }
    }


}
