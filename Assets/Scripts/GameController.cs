﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public GameObject plantFood;
    public float foodSpawnStartWait;
    public float foodSpawnWait;
    public int waveCount;
    public int waveDurationInSeconds;
    public int firstValidWave;
    public int flowerInitialSeconds;
    public int flowerMaxSeconds;
    public int flowerWiltThresholdInSeconds;
    public LevelManager Manager;
    public Button MenuButton;
    public Transform PlayerTransform;
    public Tilemap levelGrid;
    public bool playerIsMoving = false;
    public Vector3 PlayerDestination;
    public Direction PlayerDirection;
    public float PlayerSpeed = 1f;
    public int CellSize = 32;
    public bool EdgeJump = true;
    private Flower flower;
    private FoodMap foodMap;
    private bool gameOver = false;
    private Slider flowerHealthSlider;

    public int snakeScore;

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    private void Start()
    {
        Manager = GameObject.Find("Level Manager")?.GetComponent<LevelManager>();
        MenuButton.onClick.AddListener(() => Manager?.OnMenu());
        PlayerDestination = PlayerTransform.position;
        flowerHealthSlider = GameObject.Find("Flower Health Slider").GetComponent<Slider>();
        foodMap = new FoodMap(waveCount, firstValidWave);
        flower = new Flower(flowerInitialSeconds, flowerMaxSeconds, flowerWiltThresholdInSeconds);
        foodMap = new FoodMap(waveCount, 2);
        flowerHealthSlider.maxValue = flower.MaxSecondsRemaining;
        flowerHealthSlider.minValue = 0f;
        flowerHealthSlider.wholeNumbers = true;
        StartCoroutine(FoodWaveController());
        StartCoroutine(SpawnFood());
        StartCoroutine(FlowerCountdownTimer());
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
        flowerHealthSlider.value = flower.SecondsRemaining;
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
                if (levelGrid.HasTile(Vector3Int.CeilToInt(PlayerTransform.position + Vector3.up * CellSize)))
                PlayerDestination = levelGrid.GetCellCenterWorld(levelGrid.WorldToCell(PlayerTransform.position + Vector3.up * CellSize));
                else if(EdgeJump)
                {
                    PlayerTransform.position = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y * -1, 0f);
                    PlayerDestination = levelGrid.GetCellCenterWorld(levelGrid.WorldToCell((PlayerTransform.position + Vector3.up * CellSize)));

                }
                break;
            case Direction.Down:
                if (levelGrid.HasTile(Vector3Int.FloorToInt(PlayerTransform.position + Vector3.down * CellSize)))
                PlayerDestination = levelGrid.GetCellCenterWorld(levelGrid.WorldToCell(PlayerTransform.position + Vector3.down * CellSize));
                else if (EdgeJump)
                {
                    PlayerTransform.position = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y * -1, 0f);
                    PlayerDestination = levelGrid.GetCellCenterWorld(levelGrid.WorldToCell(PlayerTransform.position));
                }
                break;
            case Direction.Left:
                if (levelGrid.HasTile(Vector3Int.FloorToInt(PlayerTransform.position + Vector3.left * CellSize)))
                    PlayerDestination = levelGrid.GetCellCenterWorld(levelGrid.WorldToCell(PlayerTransform.position + Vector3.left * CellSize));
                else if (EdgeJump)
                {
                    PlayerTransform.position = new Vector3(PlayerTransform.position.x * -1, PlayerTransform.position.y, 0f);
                    PlayerDestination = levelGrid.GetCellCenterWorld(levelGrid.WorldToCell(PlayerTransform.position));
                }
                break;
            case Direction.Right:
                if (levelGrid.HasTile(Vector3Int.CeilToInt(PlayerTransform.position + Vector3.right * CellSize)))
                    PlayerDestination = levelGrid.GetCellCenterWorld(levelGrid.WorldToCell(PlayerTransform.position + Vector3.right * CellSize));
                else if (EdgeJump)
                {
                    PlayerTransform.position = new Vector3(PlayerTransform.position.x * -1, PlayerTransform.position.y, 0f);
                    PlayerDestination = levelGrid.GetCellCenterWorld(levelGrid.WorldToCell(PlayerTransform.position));
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        playerIsMoving = true;
    }


    public void GoToOppositeEdge(Vector3 directionalVector)
    {

        PlayerTransform.position = new Vector3(PlayerTransform.position.x * directionalVector.x,
            PlayerTransform.position.y * directionalVector.y, 0f);
        PlayerDestination = levelGrid.GetCellCenterWorld(levelGrid.WorldToCell(PlayerTransform.position));



    }

    public void AddSecondsToFlower(int secondsToAdd)
    {
        Debug.Log("Adding " + secondsToAdd + " seconds to flower");
        flower.IncrementFlowerSeconds(secondsToAdd);
        Debug.Log("Flower total is now " + flower.SecondsRemaining);
    }

    IEnumerator FoodWaveController()
    {
        Debug.Log($"FoodWaveController is running");

        yield return new WaitForSeconds(foodSpawnStartWait);

        for (int wave = 0; wave < (foodMap.foodWaveCount - 1); wave++)
        {
            yield return new WaitForSeconds(waveDurationInSeconds);

            foodMap.IncrementCurrentWave();
        }
    }

    IEnumerator SpawnFood()
    {
        yield return new WaitForSeconds(foodSpawnStartWait);

        while (!gameOver)
        {
            if (foodMap.CurrentWaveHasUnoccupiedPositions())
            {
                Vector2Int position = foodMap.GetCurrentWaveRandomUnoccupiedPosition();

                Debug.Log($"Spawning at {position.x}, {position.y}");

                InstantiateGameObjectAtPosition(plantFood, position);

                foodMap.MarkOccupied(position);
            }

            yield return new WaitForSeconds(foodSpawnWait);
        }

        Debug.Log("Game is over. No more food will be spawned.");
    }

    IEnumerator FlowerCountdownTimer()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(1);
            flower.DecrementFlowerSeconds(1);
            if (flower.CurrentHealth == Flower.Health.Dead)
            {
                Debug.Log("Flower has died. GAME OVER!");
                gameOver = true;
            }
        }
    }

    public void AddToScore(int pointsToAdd)
    {
        snakeScore += pointsToAdd;
        Debug.Log(snakeScore);
    }

    public void ConsumeItem(Vector2 position)
    {
        foodMap.MarkUnoccupied(position);
    }

    private void InstantiateGameObjectAtPosition(GameObject gameObject, Vector2Int position)
    {
        Vector3Int newPosition = new Vector3Int(position.x, position.y, 0);

        Instantiate(
            gameObject,
            levelGrid.GetCellCenterWorld(newPosition),//newPosition,
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

    public bool HasUnoccupiedPositions(FoodMap foodMap)
    {
        List<int> fixedRows = new List<int> { minRow, maxRow };
        List<int> fixedColumns = new List<int> { minColumn, maxColumn };

        foreach (var row in fixedRows)
        {
            for (int column = minColumn; column <= maxColumn; ++column)
            {
                if (!foodMap.IsOccupied(new Vector2Int(column, row)))
                {
                    return true;
                }
            }
        }

        foreach (var column in fixedColumns)
        {
            for (int row = minRow + 1; row < maxRow; ++row)
            {
                if (!foodMap.IsOccupied(new Vector2Int(column, row)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsPositionValid(int column, int row)
    {
        if (((row == minRow) || (row == maxRow)) && (column >= minColumn) && (column <= maxColumn))
            return true;
        else if (((column == minColumn) || (column == maxColumn)) && (row >= minRow) && (row <= maxRow))
            return true;

        return false;
    }

    public Vector2Int GetRandomPosition()
    {
        int column;
        int row;

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

        return new Vector2Int(column, row);
    }

    private bool RandomBool() => (Random.Range(0, 2) == 1);
}

public class FoodMap
{
    public readonly int foodWaveCount;

    private int CurrentWave;

    private readonly bool[,] foodMap;
    private readonly List<FoodWave> foodwaves = new List<FoodWave>();

    private readonly int WaveCount;

    private readonly int columnCount;
    private readonly int rowCount;

    public FoodMap(int newWaveCount, int firstValidWave)
    {
        WaveCount = newWaveCount;
        foodWaveCount = WaveCount;

        // Set CurrentWave to start at the selected first valid wave.
        CurrentWave = firstValidWave - 1;

        // Bounds check CurrentWave.
        // Not: CurrentWave is 0-based, but firstValidWave is 1-based.
        if (CurrentWave < 0)
        {
            CurrentWave = 0;
        }

        if (CurrentWave >= WaveCount)
        {
            CurrentWave = WaveCount - 1;
        }

        Debug.Log($"Current wave is initialized to '{CurrentWave}'");

        CreateFoodWaves();

        columnCount = WaveCount * 2 + 1; ;
        rowCount = columnCount;

        foodMap = new bool[columnCount, rowCount];

        for (int column = 0; column < columnCount; ++column)
        {
            for (int row = 0; row < rowCount; ++row)
            {
                foodMap[column, row] = false;
            }
        }
    }

    public void IncrementCurrentWave()
    {
        // Make sure the currentWave does not exceed the waveCount.
        if (CurrentWave < (WaveCount - 1))
        {
            CurrentWave++;
        }

        Debug.Log($"Current wave is '{CurrentWave}'");
    }

    public bool CurrentWaveHasUnoccupiedPositions()
    {
        return foodwaves[CurrentWave].HasUnoccupiedPositions(this);
    }

    public Vector2Int GetCurrentWaveRandomUnoccupiedPosition()
    {
        Vector2Int position;

        // Find an unoccupied position to spawn the plant food.
        do
        {
            position = foodwaves[CurrentWave].GetRandomPosition();
        } while (IsOccupied(position));

        return position;
    }

    public bool IsOccupied(Vector2Int position)
    {
        int column = position.x + (columnCount / 2);
        int row = position.y + (rowCount / 2);

        return IsOccupied(column, row);
    }

    public bool IsOccupied(int column, int row) => foodMap[column, row];

    public void MarkOccupied(Vector2 position)
    {
        int column = (int)position.x + (columnCount / 2);
        int row    = (int)position.y + (rowCount / 2);

        MarkOccupied(column, row);
    }

    public void MarkOccupied(int column, int row) => foodMap[column, row] = true;

    public void MarkUnoccupied(Vector2 position)
    {
        int column = (int)position.x + (columnCount / 2);
        int row = (int)position.y + (rowCount / 2);

        MarkUnoccupied(column, row);
    }

    public void MarkUnoccupied(int column, int row) => foodMap[column, row] = false;

    public int GetFoodWaveMaxItems(int wave) => foodwaves[wave].maxFoodItems;

    private void CreateFoodWaves()
    {
        for (int i = 1; i <= foodWaveCount; ++i)
        {
            FoodWave foodwave = new FoodWave(i);
            foodwaves.Add(foodwave);
        }
    }
}

public class Flower
{
    public int SecondsRemaining
    { get; set; }

    public int MaxSecondsRemaining
    { get; private set; }

    public int WiltThreshold
    { get; private set; }

    public enum Health
    {
        MaxHealth,
        Alive,
        Wilting,
        Dead
    }




    public Health CurrentHealth
    { get; private set; }

    private static readonly object lockobj = new object();
    
    public Flower(
        int initialSecondsRemaining, 
        int newMaxSecondsRemaining,
        int newWiltThreshold)
    {
        SecondsRemaining = initialSecondsRemaining;
        MaxSecondsRemaining = newMaxSecondsRemaining;
        WiltThreshold = newWiltThreshold;
        UpdateCurrentHealth();
    }

    public void IncrementFlowerSeconds(int numberOfSeconds)
    {
        lock (lockobj)
        {
            SecondsRemaining = Math.Min(SecondsRemaining + numberOfSeconds, MaxSecondsRemaining);
            UpdateCurrentHealth();
        }
    }

    public void DecrementFlowerSeconds(int numberOfSeconds)
    {
        lock (lockobj)
        {
            SecondsRemaining -= numberOfSeconds;
            UpdateCurrentHealth();
        }
    }

    private void UpdateCurrentHealth()
    {
        if (CurrentHealth == Health.Dead)
        {
            // There is no escaping Death.
            return;
        }
        if (SecondsRemaining <= 0)
        {
            CurrentHealth = Health.Dead;
            Debug.Log("Flower has died.");
            return;
        }
        if (SecondsRemaining <= WiltThreshold)
        {
            CurrentHealth = Health.Wilting;
            Debug.Log("Flower is wilting.");
            return;
        }
        if (SecondsRemaining == MaxSecondsRemaining)
        {
            CurrentHealth = Health.MaxHealth;
            Debug.Log("Flower is at max health.");
            return;
        }

        CurrentHealth = Health.Alive;
    }
}