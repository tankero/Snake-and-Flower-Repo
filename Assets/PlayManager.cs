using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class PlayManager : MonoBehaviour
{
    // Start is called before the first frame update
    public LevelManager Manager;
    public Button MenuButton;
    public Transform PlayerTransform;
    public Tilemap levelGrid;
    public bool playerIsMoving = false;
    public Vector3 PlayerDestination;

    public float PlayerSpeed = 1f;
    public int CellSize = 32;

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }


    void Start()
    {
        Manager = GameObject.Find("Level Manager")?.GetComponent<LevelManager>();
        MenuButton.onClick.AddListener(() => Manager?.OnMenu());
        PlayerDestination = PlayerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerIsMoving)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                MovePlayer(Direction.Up);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                MovePlayer(Direction.Down);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                MovePlayer(Direction.Left);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                MovePlayer(Direction.Right);
            }
        }
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(PlayerTransform.position, PlayerDestination) <= 0.1f)
        {
            PlayerTransform.position = levelGrid.GetCellCenterWorld(Vector3Int.FloorToInt(PlayerDestination));
            playerIsMoving = false;

        }
        else
        {
            PlayerTransform.position = Vector3.Lerp(PlayerTransform.position, PlayerDestination,
                PlayerSpeed * Time.fixedDeltaTime);
        }
    }

    public void MovePlayer(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                PlayerDestination = PlayerTransform.position + (Vector3.up * CellSize);
                break;
            case Direction.Down:
                PlayerDestination = PlayerTransform.position + (Vector3.down * CellSize);
                break;
            case Direction.Left:
                PlayerDestination = PlayerTransform.position + (Vector3.left * CellSize);
                break;
            case Direction.Right:
                PlayerDestination = PlayerTransform.position + (Vector3.right * CellSize);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        playerIsMoving = true;
    }

    public void ResetMovement()
    {
        PlayerTransform.position = levelGrid.GetCellCenterWorld(Vector3Int.FloorToInt(PlayerTransform.position));
        PlayerDestination = PlayerTransform.position;
        playerIsMoving = false;
    }
}
