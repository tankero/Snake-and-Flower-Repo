using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public GameObject plantFood;
    public float startWait;
    public float spawnWait;
    public float waveWait;
    public int waveCount;
    public LevelManager Manager;
    public Button MenuButton;
    public Transform PlayerTransform;
    public Tilemap levelGrid;
    public bool playerIsMoving = false;
    public Vector3 PlayerDestination;

    public float PlayerSpeed = 1f;
    public int CellSize = 32;
    private readonly List<FoodWave> foodwaves = new List<FoodWave>();
    [SerializeField]
    private int score;

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private void Start()
    {
        CreateFoodWaves();
        StartCoroutine(SpawnFood());
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

    private void CreateFoodWaves()
    {
        for (int i = 1; i <= waveCount; ++i)
        {
            FoodWave foodwave = new FoodWave(i);
            foodwaves.Add(foodwave);
        }
    }

    IEnumerator SpawnFood()
    {
        FoodMap foodmap = new FoodMap(waveCount);

        yield return new WaitForSeconds(startWait);
        for (int i = 0; i < foodwaves.Count; i++)
        {
            Debug.Log($"Spawning at wave #{i}");

            for (int j = 0; j < foodwaves[i].maxFoodItems ; j++)
            {
                Vector2Int position;

                do
                {
                    position = foodwaves[i].GetRandomPosition();

                    Debug.Log($"position = ({position.x},{position.y})");
                } while (foodmap.IsOccupied(position));
                
                Debug.Log($"Spawning at {position.x}, {position.y}");

                // This code should be moved into a method in FoodMap
                {
                    Vector3Int newPosition = new Vector3Int(position.x, position.y, 0);

                    Instantiate(
                        plantFood,
                        levelGrid.GetCellCenterWorld(newPosition),
                        transform.rotation);

                    foodmap.MarkOccupied(position);
                }

                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waveWait);
        }
    }

    public void AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        Debug.Log(score);
    }
}

public class FoodWave
{
    public int maxFoodItems;
    private int minRow;
    private int maxRow;
    private int minColumn;
    private int maxColumn;

    public FoodWave(int wave)
    {
        maxFoodItems = wave*8;
        minRow = -wave;
        maxRow = wave;
        minColumn = -wave;
        maxColumn = wave;
    }


    public Vector2Int GetRandomPosition()
    {
        int row;
        int column;

        // For each position within a wave,
        // either the row or the column must fixed to the min or max value.
        // The element that is not fixed can be any value between the min and the max.

        if (RandomBool())
        {
            row = RandomBool() ? minRow : maxRow;

            column = Random.Range(minColumn, maxColumn + 1);
        }
        else
        {
            column = RandomBool() ? minColumn : maxColumn;

            row = Random.Range(minRow, maxRow + 1);
        }

        return new Vector2Int(row, column);
    }

    private bool RandomBool() => (Random.Range(0, 2) == 1);
}

public class FoodMap
{
    private bool[,] foodMap;

    private int rowCount;
    private int columnCount;
    
    public FoodMap(int waveCount)
    {
        rowCount = waveCount*2 + 1;
        columnCount = rowCount;

        foodMap = new bool[rowCount, columnCount];

        for (int i = 0; i < rowCount; ++i)
        {
            for (int j = 0; j < columnCount; ++j)
            {
                foodMap[i, j] = false;
            }
        }
    }

    public bool IsOccupied(Vector2 position)
    {

        int row = (int)position.x + (rowCount/2);
        int column = (int)position.y + (columnCount/2);

        return IsOccupied(row, column);
    }

    public bool IsOccupied(int row, int column) => foodMap[row, column];

    public void MarkOccupied(Vector2 position)
    {
        int row = (int)position.x + (rowCount/2);
        int column = (int)position.y + (columnCount/2);

        MarkOccupied(row, column);
    }

    public void MarkOccupied(int row, int column) => foodMap[row, column] = true;
}