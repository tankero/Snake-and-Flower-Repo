using UnityEngine;
using System.Collections.Generic;

public class FoodWave
{
    public int maxFoodItems;
    private readonly int minRow;
    private readonly int maxRow;
    private readonly int minColumn;
    private readonly int maxColumn;

    public FoodWave(int wave)
    {
        maxFoodItems = wave * 8;
        minRow = -wave;
        maxRow = wave;
        minColumn = -wave;
        maxColumn = wave;
    }

    public bool HasValidUnoccupiedPositions(FoodMap foodMap)
    {
        List<int> fixedRows = new List<int> { minRow, maxRow };
        List<int> fixedColumns = new List<int> { minColumn, maxColumn };

        foreach (var row in fixedRows)
        {
            for (int column = minColumn; column <= maxColumn; ++column)
            {
                Vector2Int position = new Vector2Int(column, row);
                if (foodMap.IsValidAndUnoccupied(position))
                {
                    return true;
                }
            }
        }

        foreach (var column in fixedColumns)
        {
            for (int row = minRow + 1; row < maxRow; ++row)
            {
                Vector2Int position = new Vector2Int(column, row);
                if (foodMap.IsValidAndUnoccupied(position))
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