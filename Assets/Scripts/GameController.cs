using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject plantFood;
    public float startWait;
    public float spawnWait;
    public float waveWait;
    public int waveCount;
   [SerializeField] int score = 0;

    private readonly List<FoodWave> foodwaves = new List<FoodWave>();

    private void Start()
    {

        CreateFoodWaves();
        StartCoroutine(SpawnFood());
        
    }

    private void CreateFoodWaves()
    {
        for (int i = 1; i <= waveCount; ++i)
        {
            FoodWave foodwave = new FoodWave(i);
            foodwaves.Add(foodwave);
        }
    }
    public void AddToScore(int pointsToAdd)
    {
    score += pointsToAdd; //adds total amount to food
    Debug.Log(score);//TODO implement system that will dropoff total score to the plant object
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
                Vector2 position;

                do
                {
                    position = foodwaves[i].GetRandomPosition();

                    Debug.Log($"position = ({position.x},{position.y})");
                } while (foodmap.IsOccupied(position));
                
                Debug.Log($"Spawning at {position.x}, {position.y}");

                Instantiate(
                    plantFood, 
                    position, 
                    transform.rotation);

                foodmap.MarkOccupied(position);

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

    public FoodWave(int wave)
    {
        maxFoodItems = wave*8;
        minRow = -wave;
        maxRow = wave;
        minColumn = -wave;
        maxColumn = wave;
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

            column = Random.Range(minColumn, maxColumn + 1);
        }
        else
        {
            column = RandomBool() ? minColumn : maxColumn;

            row = Random.Range(minRow, maxRow + 1);
        }

        return new Vector2(row, column);
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

