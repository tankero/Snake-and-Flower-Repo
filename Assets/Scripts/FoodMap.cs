using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FoodMap
{
    public readonly int foodWaveCount;
    public int CurrentWave
    { get; private set; }

    private readonly Tilemap levelGrid;
    

    private readonly bool[,] foodMap;
    private readonly List<FoodWave> foodwaves = new List<FoodWave>();

    private readonly int WaveCount;

    private readonly int columnCount;
    private readonly int rowCount;

    public FoodMap(
        Tilemap newLevelGrid,
        int newWaveCount,
        int firstValidWave)
    {
        levelGrid = newLevelGrid;
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

        Debug.Log($"Current food wave is initialized to '{CurrentWave + 1}'");

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

    public bool AtLastWave()
    {
        return CurrentWave == (WaveCount - 1);
    }

    public void IncrementCurrentWave()
    {
        // Make sure the currentWave does not exceed the waveCount.
        if (CurrentWave < (WaveCount - 1))
        {
            CurrentWave++;
        }

        Debug.Log($"******* Current food wave is '{CurrentWave + 1}' *******");
    }

    public bool CurrentWaveHasValidUnoccupiedPositions()
    {
        return foodwaves[CurrentWave].HasValidUnoccupiedPositions(this);
    }

    public Vector2Int GetCurrentWaveRandomUnoccupiedPosition()
    {
        return GetWaveRandomUnoccupiedPosition(CurrentWave);
    }

    public Vector2Int GetWaveRandomUnoccupiedPosition(int wave)
    {
        Vector2Int position;

        bool validUnoccupiedPositionFound = false;

        // Find an unoccupied position to spawn the plant food.
        do
        {
            position = foodwaves[wave].GetRandomPosition();

            if (IsValidAndUnoccupied(position))
            {
                validUnoccupiedPositionFound = true;
            }

        } while (!validUnoccupiedPositionFound);

        return position;
    }

    public bool IsValidAndUnoccupied(Vector2Int position)
    {
        return (IsValid(position) && !IsOccupied(position));
    }

    private bool IsValid(Vector2Int position)
    {
        Vector3Int gridPosition = new Vector3Int(position.x, position.y, 0);

        return levelGrid.HasTile(gridPosition);
    }

    private bool IsOccupied(Vector2Int position)
    {
        int column = position.x + (columnCount / 2);
        int row = position.y + (rowCount / 2);

        return IsOccupied(column, row);
    }

    private bool IsOccupied(int column, int row) => foodMap[column, row];

    public void MarkOccupied(Vector2 position)
    {
        int column = (int)position.x + (columnCount / 2);
        int row = (int)position.y + (rowCount / 2);

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