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

    public Vector3 GetNewEnemyDirection()
    {

        int x = RandomBool() ? 1 : 0;
        int y = RandomBool() ? 1 : 0;
        while (x + y == 0)
        {
            x = RandomBool() ? 1 : 0;
            y = RandomBool() ? 1 : 0;
        }
        x *= RandomBool() ? 1 : -1;
        y *= RandomBool() ? 1 : -1;

        return new Vector3(x, y, 0);

    }
}