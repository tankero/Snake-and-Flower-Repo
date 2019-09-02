using UnityEngine;
using UnityEditor;

public class EnemySpawner
{
    private float horizontalEnemyLifetime;
    private float verticalEnemyLifetime;
    private int currentEnemiesSpawned;
    private int minRow = -5;
    private int maxRow = 5;
    private int minColumn = -9;
    private int maxColumn = 9;

    public EnemySpawner(
        float newHorizontalEnemyLifetime,
        float newVerticalEnemyLifetime)
    {
        currentEnemiesSpawned = 0;

        horizontalEnemyLifetime = newHorizontalEnemyLifetime;
        verticalEnemyLifetime = newVerticalEnemyLifetime;
}

    public bool ShouldSpawnEnemy(int CurrentWave)
    {
        return (currentEnemiesSpawned < CurrentWave);
    }

    public Vector3 GetNewEnemySpawnPosition()
    {
        Vector3 returnValue = GetRandomPosition();

        return returnValue;
    }

    public void OnEnemySpawned()
    {
        currentEnemiesSpawned++;
    }

    public void OnEnemyDespawned()
    {
        currentEnemiesSpawned--;
    }

    public Vector3 GetRandomPosition()
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

        return new Vector3(column, row, 0);
    }

    private bool RandomBool() => (Random.Range(0, 2) == 1);

    public Vector3 GetNewEnemyDirection(Vector3 spawnPosition)
    {
        // The enemy's direction will be based on their spawn position.
        // The idea is that they should move directly across the board.

        if (spawnPosition.x == minColumn)
        {
            return new Vector3(1, 0, 0);
        }

        if (spawnPosition.x == maxColumn)
        {
            return new Vector3(-1, 0, 0);
        }

        if (spawnPosition.y == minRow)
        {
            return new Vector3(0, 1, 0);
        }

        Debug.Assert(spawnPosition.y == maxRow);
        return new Vector3(0, -1, 0);
    }

    public float GetNewEnemyLifetime(Vector3 spawnPosition)
    {
        if ((spawnPosition.x == minColumn) || (spawnPosition.x == maxColumn))
        {
            return horizontalEnemyLifetime;
        }

        Debug.Assert((spawnPosition.y == maxRow) || (spawnPosition.y == minRow));
        return verticalEnemyLifetime;
    }
}