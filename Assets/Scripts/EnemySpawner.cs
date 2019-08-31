using UnityEngine;
using UnityEditor;

public class EnemySpawner
{
    private int currentEnemiesSpawned;
    private int minRow = -5;
    private int maxRow = 5;
    private int minColumn = -9;
    private int maxColumn = 9;

    public EnemySpawner()
    {
        currentEnemiesSpawned = 0;
    }

    public bool ShouldSpawnEnemy(int CurrentWave)
    {
        return (currentEnemiesSpawned < CurrentWave);
    }

    public Vector2Int GetNewEnemySpawnPosition()
    {
        Vector2Int returnValue = GetRandomPosition();

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