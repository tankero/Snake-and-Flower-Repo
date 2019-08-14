using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject plantFood;
    public float startWait;
    public float spawnWait;
    public float waveWait;

    private readonly List<FoodWave> foodwaves = new List<FoodWave>();

    private void Start()
    {
        CreateFoodWaves();
        StartCoroutine(SpawnFood());
    }

    private void CreateFoodWaves()
    {
        FoodWave foodwave1 = new FoodWave(8, -1, 1, -1, 1);
        foodwaves.Add(foodwave1);

        FoodWave foodwave2 = new FoodWave(16, -2, 2, -2, 2);
        foodwaves.Add(foodwave2);

        FoodWave foodwave3 = new FoodWave(24, -3, 3, -3, 3);
        foodwaves.Add(foodwave3);
    }

    IEnumerator SpawnFood()
    {
        yield return new WaitForSeconds(startWait);
        for (int i = 0; i < foodwaves.Count; i++)
        {
            Debug.Log($"Spawning at wave #{i}");

            for (int j = 0; j < foodwaves[i].maxFoodItems ; j++)
            {
                Vector2 position = foodwaves[i].GetRandomPosition();

                Debug.Log($"Spawning at {position.x}, {position.y}");

                Instantiate(
                    plantFood, 
                    position, 
                    transform.rotation);

                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waveWait);
        }
    }
}

public class FoodWave
{
    public int maxFoodItems;
    private int minRow;
    private int maxRow;
    private int minColumn;
    private int maxColumn;

    public FoodWave(
        int newMaxFoodItems,
        int newMinRow,
        int newMaxRow,
        int newMinColumn,
        int newMaxColumn)
    {
        maxFoodItems = newMaxFoodItems;
        minRow = newMinRow;
        maxRow = newMaxRow;
        minColumn = newMinColumn;
        maxColumn = newMaxColumn;
    }


    public Vector2 GetRandomPosition()
    {
        int row;
        int column;

        // For each position within a wave,
        // either the row or the column must fixed to the min or max value.
        // The element that is not fixed can be any value between the min and the max.

        if (RandomBool())
        {
            row = RandomBool() ? minRow : maxRow;

            column = Random.Range(minColumn, maxColumn);
            Debug.Log($"ColumnA: {column}");
        }
        else
        {
            column = RandomBool() ? minColumn : maxColumn;
            Debug.Log($"ColumnB: {column}");

            row = Random.Range(minRow, maxRow);
        }

        Debug.Log($"Column: {column}");

        return new Vector2(row, column);
    }

    private bool RandomBool()
    {
        int randomValue = Random.Range(0, 2);
        Debug.Log($"randomValue: {randomValue}");
        return randomValue == 1;
    }

}
