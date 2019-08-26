using UnityEngine;

public class Snake
{
    public int CurrentFoodCount
    { get; private set; }

    private static readonly object lockobj = new object();

    public Snake()
    {
        CurrentFoodCount = 0;
    }

    public void IncrementCurrentFoodCount(int amount)
    {
        lock (lockobj)
        {
            CurrentFoodCount += amount;
            Debug.Log($"snake CurrentFoodCount = {CurrentFoodCount}");
        }
    }

    public void DerementCurrentFoodCount(int amount)
    {
        lock (lockobj)
        {
            CurrentFoodCount -= amount;
            Debug.Log($"snake CurrentFoodCount = {CurrentFoodCount}");
        }
    }
}