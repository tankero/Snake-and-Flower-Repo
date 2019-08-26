using UnityEngine;

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
            SecondsRemaining = Mathf.Min(SecondsRemaining + numberOfSeconds, MaxSecondsRemaining);
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

        Debug.Log($"Flower SecondsRemaining: {SecondsRemaining}.");

        if (SecondsRemaining <= 0)
        {
            CurrentHealth = Health.Dead;
            Debug.Log("Flower has died.");
            return;
        }
        if (SecondsRemaining <= WiltThreshold)
        {
            CurrentHealth = Health.Wilting;
            //flowerRenderer.sprite = wiltstage1; //this wasn't working
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
