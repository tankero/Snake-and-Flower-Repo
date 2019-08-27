using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
    public int flowerSecondsGainedPerFood;

    public GameObject rockObstacle;
    public GameObject logObstacle;
    public int obstaclesPerFoodWave;

    public LevelManager Manager;
    public Button MenuButton;
    public Transform PlayerTransform;
    public Tilemap levelGrid;
    public bool playerIsMoving = false;
    public Vector3 PlayerDestination;
    

    public float PlayerSpeed = 1f;
    public int CellSize = 32;
    public bool EdgeJump = true;

    public Flower flower;
    private FoodMap foodMap;
    private Snake snake;
    private bool gameOver = false;

    private Slider flowerHealthSlider;

    public int snakeScore;

    public Image background;
    public float backgroundFade = 5f;
    public GameObject GameOverContainer;

    public bool gameIsEnding = false;

    public object Health { get; internal set; }

    
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
        
        foodMap = new FoodMap(levelGrid, waveCount, firstValidWave);
        // This needs to be called after the foodMap is created.
        SpawnObstacles();

        flower = new Flower(flowerInitialSeconds, flowerMaxSeconds, flowerWiltThresholdInSeconds);

        snake = new Snake();

        flowerHealthSlider.maxValue = flower.MaxSecondsRemaining;
        flowerHealthSlider.minValue = 0f;
        flowerHealthSlider.wholeNumbers = true;

        StartCoroutine(FoodWaveController());
        StartCoroutine(SpawnFood());
        StartCoroutine(FlowerCountdownTimer());
        var fixedColor = background.color;
        fixedColor.a = 1;
        background.color = fixedColor;
        background.CrossFadeAlpha(0f, 0f, true);



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
        if (gameOver)
        {
            return;
        }
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
        

    IEnumerator GameOver()
    {
        background.CrossFadeAlpha(1f, backgroundFade, true);
        gameIsEnding = true;
        yield return new WaitForSeconds(backgroundFade);
        GameOverContainer.SetActive(true);
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

        while (!foodMap.AtLastWave())
        {
            yield return new WaitForSeconds(waveDurationInSeconds);

            if (gameOver)
            {
                Debug.Log("Game is over. Wave controller shutting down.");
                break;
            }

            foodMap.IncrementCurrentWave();
        }

        Debug.Log($"FoodWaveController is done.");
    }

    IEnumerator SpawnFood()
    {
        yield return new WaitForSeconds(foodSpawnStartWait);

        while (!gameOver)
        {
            if (foodMap.CurrentWaveHasValidUnoccupiedPositions())
            {
                Vector2Int position = foodMap.GetCurrentWaveRandomUnoccupiedPosition();

                Debug.Log($"Spawning food at {position.x}, {position.y}");

                InstantiateGameObjectAtPosition(plantFood, position);

                foodMap.MarkOccupied(position);
            }

            yield return new WaitForSeconds(foodSpawnWait);
        }

        if(!gameIsEnding)
        StartCoroutine(GameOver());
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

    public void OnSnakeCollisionWithFood(Vector2Int position)
    {
        // When the snake collides with a plant food, 
        // we want to increment the current food count of the snake.
        snake.IncrementCurrentFoodCount(1);

        foodMap.MarkUnoccupied(position);
    }

    public void AddToScore(int pointsToAdd)
    {
        snakeScore += pointsToAdd;
    }

    public void OnSnakeCollisionWithFlower()
    {
        // When the snake collides with the flower,
        // we want to convert the food carried by the snake 
        // into life for the flower (in the form of time).
        int foodCountToTransfer = snake.CurrentFoodCount;
        snake.DerementCurrentFoodCount(foodCountToTransfer);

        int flowerTimeIncreaseFromFood = flowerSecondsGainedPerFood * foodCountToTransfer;
        flower.IncrementFlowerSeconds(flowerTimeIncreaseFromFood);
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

    public void PlayAgain()
    {
        SceneManager.LoadScene(1);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void SpawnObstacles()
    {
        for (int wave = 0; wave < foodMap.foodWaveCount; ++wave)
        {
            for (int i = 0; i < obstaclesPerFoodWave; ++i)
            {
                Vector2Int position = foodMap.GetWaveRandomUnoccupiedPosition(wave);

                foodMap.MarkOccupied(position);

                if (RandomBool())
                {
                    InstantiateGameObjectAtPosition(logObstacle, position);
                }
                else
                {
                    InstantiateGameObjectAtPosition(rockObstacle, position);
                }
            }
        }
    }

    private bool RandomBool() => (UnityEngine.Random.Range(0, 2) == 1);
}