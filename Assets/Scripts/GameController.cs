using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = System.Random;

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
    public int enemySpawnStartWait;
    public int enemySpawnWait;

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

    public TMP_Text ScoreDisplay;

    public GameObject EnemyContainer;

    public Image background;
    public float backgroundFade = 5f;
    public GameObject GameOverContainer;

    public bool gameIsEnding = false;

    public object Health { get; internal set; }

    public List<GameObject> EnemyList;

    public int EnemyPoolLimit = 5;

    private EnemySpawner enemySpawner;

    public int playerScore;

    public GameObject EnemyPrefab;
    public float EnemyMovementCadenceSetting = 1f;
    public float EnemyLifetimeSetting = 25f;

    private bool snakeStunned = false;
    public float SnakeStunTime = 3;

    private void Awake()
    {

    }

    private void Start()
    {
        Manager = GameObject.Find("Level Manager")?.GetComponent<LevelManager>();
        MenuButton.onClick.AddListener(() => Manager?.OnMenu());
        ScoreDisplay = GameObject.Find("Score Display").GetComponent<TMP_Text>();

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

        playerScore = 0;

        StartCoroutine(FoodWaveController());
        StartCoroutine(SpawnFood());
        StartCoroutine(FlowerCountdownTimer());
        var fixedColor = background.color;
        fixedColor.a = 1;
        background.color = fixedColor;
        background.CrossFadeAlpha(0f, 0f, true);
        GameOverContainer.SetActive(false);

        enemySpawner = new EnemySpawner();

        StartCoroutine(SpawnEnemies());

        for (int i = 0; i < EnemyPoolLimit; i++)
        {
            // This line throws an exception
            var temp = Instantiate(EnemyPrefab, Vector3.zero, Quaternion.identity, EnemyContainer.transform);

            temp.SetActive(false);
            EnemyList.Add(temp);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (gameOver)
        {
            return;
        }
        if (!playerIsMoving && !snakeStunned)
        {
            MovePlayer();
        }

        foreach (var enemy in EnemyList)
        {
            if (!enemy.activeInHierarchy)
            {
                continue;
            }
            var script = enemy.GetComponent<EnemyScript>();
            script.Lifetime -= Time.fixedDeltaTime;
            if (script.Lifetime <= 0)
            {
                enemySpawner.OnEnemyDespawned();
                enemy.SetActive(false);
                
            }
            if (!script.IsMoving && Time.time - script.LastMoveTime >= script.Cadence)
            {
                MoveEnemy(enemy);
                script.LastMoveTime = Time.time;
            }

            if (Vector3.Distance(enemy.transform.position, script.Destination) <= 0.1f)
            {
                enemy.transform.position =
                    levelGrid.GetCellCenterWorld(Vector3Int.FloorToInt(script.Destination));
                script.IsMoving = false;
            }
            else
            {
                enemy.transform.position = Vector3.Lerp(enemy.transform.position, script.Destination, script.Speed * Time.fixedDeltaTime);
                script.IsMoving = true;
            }
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

        ScoreDisplay.text = playerScore.ToString().PadLeft(6, '0');

    }

    private bool TestForObstacles(Vector3 startLocation, Vector3 endLocation)
    {
        LayerMask mask = LayerMask.GetMask("Obstacle");
        Debug.Log($"Testing direction:{endLocation - startLocation}");


        var hit = Physics2D.Raycast(startLocation, endLocation - startLocation, CellSize, mask);

        if (hit.collider != null)
        {
            Debug.Log("Yep, that's an obstacle");
            return true;
        }

        return false;
    }

    public void MoveEnemy(GameObject enemy)
    {
        var script = enemy.GetComponent<EnemyScript>();
        var movementInput = script.MovementPattern;

        if (levelGrid.HasTile(Vector3Int.CeilToInt(enemy.transform.position + movementInput)))
        {
            script.Destination = levelGrid.WorldToCell(enemy.transform.position + movementInput);
        }
        else
        {
            if (levelGrid.HasTile(Vector3Int.CeilToInt(enemy.transform.position) +
                                  new Vector3Int(Mathf.CeilToInt(movementInput.x), 0, 0)))
            {
                enemy.transform.position = new Vector3(enemy.transform.position.x + CellSize,
                    enemy.transform.position.y * -1, 0f);
                script.Destination = enemy.transform.position;
                return;
            }
            if (levelGrid.HasTile(Vector3Int.CeilToInt(enemy.transform.position) +
                                  new Vector3Int(0, Mathf.CeilToInt(movementInput.y), 0)))
            {
                enemy.transform.position = new Vector3(enemy.transform.position.x * -1,
                    enemy.transform.position.y + CellSize, 0f);
                script.Destination = enemy.transform.position;
            }
        }



    }



    public void MovePlayer()
    {
        var axis = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f) * CellSize;
        var axisY = new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f) * CellSize;
        var axisX = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f) * CellSize;
        if (Math.Abs(Input.GetAxisRaw("Vertical")) > 0.0001)
        {

            if (levelGrid.HasTile(Vector3Int.CeilToInt(PlayerTransform.position + axisY)))
            {

                if (TestForObstacles(PlayerTransform.position, levelGrid.GetCellCenterWorld(
                    levelGrid.WorldToCell(PlayerTransform.position + axisY))))
                {
                    return;
                }
                PlayerDestination =
                        levelGrid.GetCellCenterWorld(
                            levelGrid.WorldToCell(PlayerTransform.position + axisY));

            }
            else if (EdgeJump)
            {
                PlayerTransform.position = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y * -1, 0f);
                PlayerDestination = levelGrid.GetCellCenterWorld(levelGrid.WorldToCell((PlayerTransform.position + axisY)));

            }
        }
        if (Math.Abs(Input.GetAxisRaw("Horizontal")) > 0.0001)
        {

            if (levelGrid.HasTile(Vector3Int.CeilToInt(PlayerTransform.position + axisX)))
            {

                if (TestForObstacles(PlayerTransform.position, levelGrid.GetCellCenterWorld(
                    levelGrid.WorldToCell(PlayerTransform.position + axisX))))
                {
                    return;
                }
                PlayerDestination =
                    levelGrid.GetCellCenterWorld(
                        levelGrid.WorldToCell(PlayerTransform.position + axisX));

            }
            else if (EdgeJump)
            {
                PlayerTransform.position = new Vector3(PlayerTransform.position.x * -1, PlayerTransform.position.y, 0f);
                PlayerDestination = levelGrid.GetCellCenterWorld(levelGrid.WorldToCell((PlayerTransform.position + axisX)));

            }
        }

        var direction = PlayerDestination - PlayerTransform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        PlayerTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        playerIsMoving = true;
    }


    IEnumerator GameOver()
    {
        background.CrossFadeAlpha(1f, backgroundFade, true);
        gameIsEnding = true;
        yield return new WaitForSeconds(backgroundFade);
        GameOverContainer.SetActive(true);
        GameObject.Find("Final Score Display").GetComponent<TMP_Text>().text = playerScore.ToString().PadLeft(6, '0');
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
                Debug.Log("Game is over. Wave Controller shutting down.");
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

        if (!gameIsEnding)
            StartCoroutine(GameOver());
        Debug.Log("Game is over. No more food will be spawned.");

    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(enemySpawnStartWait);

        while (!gameOver)
        {
            // For now, the current food wave will double as the enemy wave.
            // If we were doing this for real, these two systems would not be related.
            // But, in the scope of a 48 hour game jam, this will do.
            if (enemySpawner.ShouldSpawnEnemy(foodMap.CurrentWave))
            {
                Vector3 position = enemySpawner.GetNewEnemySpawnPosition();

                Vector3 direction = enemySpawner.GetNewEnemyDirection();

                Debug.Log($"Spawning enemy at {position.x}, {position.y}");

                var enemy = EnemyList.FirstOrDefault(e => !e.activeInHierarchy);
                if (enemy == null)
                {
                    enemy = Instantiate(EnemyPrefab, Vector3.zero, Quaternion.identity, EnemyContainer.transform);
                    EnemyList.Add(enemy);
                }

                
                enemy.GetComponent<EnemyScript>().Initialize(position, direction, EnemyMovementCadenceSetting, EnemyLifetimeSetting);
                enemySpawner.OnEnemySpawned();
            }

            yield return new WaitForSeconds(enemySpawnWait);
        }

        Debug.Log("Game is over. No more enemies will be spawned.");
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

    IEnumerator StunSnakeTimer()
    {
        snakeStunned = true;
        PlayerTransform.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(SnakeStunTime);
        PlayerTransform.GetComponent<SpriteRenderer>().color = Color.white;
        snakeStunned = false;
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
        // We don't want to spawn an obstacle on the snake's starting position.
        Vector2Int snakeStartingPosition = new Vector2Int(0, -5);
        foodMap.MarkOccupied(snakeStartingPosition);

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

        // Now that we are done spawning obstacles, 
        // we can clear the snake's starting position. 
        // This will allow food to spawn there.
        foodMap.MarkUnoccupied(snakeStartingPosition);
    }

    private bool RandomBool() => (UnityEngine.Random.Range(0, 2) == 1);

    public void StunSnake()
    {
        
        snakeScore = 0;
        StartCoroutine(StunSnakeTimer());
    }
}