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

    [SerializeField]
    public static int snakeScore;

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private void Start()
    {
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

    IEnumerator SpawnFood()
    {
        FoodMap foodmap = new FoodMap(waveCount);

        yield return new WaitForSeconds(startWait);
        for (int wave = 0; wave < foodmap.FoodWaveCount; wave++)
        {
            Debug.Log($"Spawning food for wave #{wave}");

            for (int i = 0; i < foodmap.GetFoodWaveMaxItems(wave); i++)
            {
                Vector2Int position;

                position = foodmap.GetRandomUnoccupiedWavePosition(wave);

                Debug.Log($"Spawning at {position.x}, {position.y}");

                InstantiateGameObjectAtPosition(plantFood, position);

                foodmap.MarkOccupied(position);

                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waveWait);
        }
    }

    public void AddToScore(int pointsToAdd)
    {
        snakeScore += pointsToAdd;
        Debug.Log(snakeScore);
    }

    private void InstantiateGameObjectAtPosition(GameObject gameObject, Vector2Int position)
    {
        Vector3Int newPosition = new Vector3Int(position.x, position.y, 0);

        Instantiate(
            gameObject,
            //levelGrid.GetCellCenterWorld(newPosition),
            newPosition,
            transform.rotation);
    }
}

public class FoodWave
{
    public int maxFoodItems;
    private readonly int minRow;
    private readonly int maxRow;
    private readonly int minColumn;
    private readonly int maxColumn;

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
    public int FoodWaveCount
    {
        get;
    }

    private readonly bool[,] foodMap;
    private readonly List<FoodWave> foodwaves = new List<FoodWave>();
    

    private readonly int rowCount;
    private readonly int columnCount;
    
    public FoodMap(int waveCount)
    {
        FoodWaveCount = waveCount;

        CreateFoodWaves();

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

    public Vector2Int GetRandomUnoccupiedWavePosition(int wave)
    {
        Vector2Int position;

        // Find an unoccupied position to spawn the plant food.
        do
        {
            position = foodwaves[wave].GetRandomPosition();
        } while (IsOccupied(position));

        return position;
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

    public int GetFoodWaveMaxItems(int wave) => foodwaves[wave].maxFoodItems;

    private void CreateFoodWaves()
    {
        for (int i = 1; i <= FoodWaveCount; ++i)
        {
            FoodWave foodwave = new FoodWave(i);
            foodwaves.Add(foodwave);
        }
    }
}