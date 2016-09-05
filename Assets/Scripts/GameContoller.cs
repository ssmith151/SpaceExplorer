using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameContoller : MonoBehaviour
{

    [Range(0, 100)]
    public int[] rockChance;
    public GameObject[] roidFieldRocks;
    [Range(0, 100)]
    public int[] enemyChance;
    public GameObject[] enemies;

    public Vector3 spawnValues;
    public Text scoreText;
    public Text restartText;
    public Text gameOverText;
    //public bool bossScene;

    public int hazardCount;
    public float spawnWait;
    public float waveWait;
    public float startWait;
    public bool menuOpen;
    public bool isSpawningLevel;
    public int levelNumber;

    public int roidNumber;
    public int maxRoidSize;
    public Score score;

    private bool restart;
    private bool gameOver;
    public bool victory;
    private MainMenuController mainMenu;

    // Use this for initialization
    void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        score = new Score();
        mainMenu = canvas.GetComponentInChildren<MainMenuController>();
        menuOpen = mainMenu.mainMenu.activeSelf;
        victory = false;
        restart = false;
        gameOver = false;
        restartText.text = "";
        gameOverText.text = "";
        CreateRoidField(roidNumber, maxRoidSize);
        //if (!bossScene)
            //StartCoroutine(SpawnWaves());
            //else
            //    StartCoroutine(BossStates());
     
        StartCoroutine(CheckForDead());
        UpdateScore();
        levelNumber = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuOpen)
            {
                mainMenu.OnInGameMenuOpen();
                menuOpen = mainMenu.mainMenu.activeSelf;
            }
            else
            {
                mainMenu.OnInGameMenuClose();
                menuOpen = mainMenu.mainMenu.activeSelf;
            }
        }
        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
    IEnumerator CheckForDead()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveWait);
            if (gameOver)
            {
                restartText.text = "Press 'R' to restart";
                restart = true;
                break;
            }
        }
    }
    public void UpdateMenuStatus()
    {
        menuOpen = mainMenu.mainMenu.activeSelf;
    }

    // #######################################################################
    // this the proceedural generation ;  it requires two integer
    // input for the number of roid and the max size of them.
    void CreateRoidField(int roidTotal, int maxSize)
    {
        // ensure that spawn rates are out of 100%
        rockChance = CheckRockChance(rockChance);
        // bool to keep player safe during spawning
        isSpawningLevel = true;
        // create one random number
        System.Random randomPer = new System.Random(Time.time.ToString().GetHashCode());
        // loop for the number of asteroids to be made
        for (int i = 0; i < roidNumber; i++)
        {
            GameObject roid = null;
            int chanceForThisLoop = randomPer.Next(1, 100);
            for(int j = 0; j < rockChance.Length; j++)
            {
                if (chanceForThisLoop < rockChance[j])
                {
                    roid = roidFieldRocks[j];
                    break;
                }
            }
            // randomize the max size
            float roidSize = Random.Range(0.5f, maxRoidSize);
            // randomize the location of the new spawnpoint, set from spawnvalues
            Vector3 spawnLocation = new Vector3(Random.Range(-spawnValues.x, spawnValues.x),
                        Random.Range(-spawnValues.y, spawnValues.y),
                        Random.Range(-spawnValues.z, spawnValues.z));
            // put the temporary shape into the world
            GameObject tempRoid = Instantiate(roid, spawnLocation, Quaternion.identity) as GameObject;
            // implement the new size
            tempRoid.transform.localScale = new Vector3(roidSize, roidSize, roidSize);
            // give it a numbered name
            tempRoid.name = "tempRoid" + i.ToString();
            // test roid position done on the temp roid game object
        }
        // gives the player their vulnerablity back
        isSpawningLevel = false;
    }
    int[] CheckRockChance(int[] rockChanceIn)
    {
        int newTotal = 0;
        foreach(int i in rockChanceIn)
        {
            newTotal += i;
        }
        if (newTotal <= 100) {
            for (int i =0; i<rockChance.Length; i++)
            {
                if(i!=0)
                    rockChanceIn[i] += rockChance[i-1];
            }
            return rockChanceIn;
        } else
        {
            int[] newChanceOut = new int[rockChanceIn.Length]; 
            for (int i = 0; i <rockChanceIn.Length; i++) {
                newChanceOut[i] = Mathf.RoundToInt(rockChanceIn[i] / newTotal);
                if (i != 0)
                    newChanceOut[i] += newChanceOut[i - 1];
            }
            return newChanceOut;
        }
    }
    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            for (int i = 0; i < hazardCount; i++)
            {
                GameObject enemy = enemies[Random.Range(0, enemies.Length)];
                Vector3 spawnLocation = new Vector3(Random.Range(-spawnValues.x, spawnValues.x),
                    Random.Range(-spawnValues.y, spawnValues.y),
                    Random.Range(-spawnValues.z, spawnValues.z));
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(enemy, spawnLocation, spawnRotation);
                yield return new WaitForSeconds(spawnWait);
            }
            //CheckObjectives();
            yield return new WaitForSeconds(waveWait);
            if (gameOver)
            {
                restartText.text = "Press 'R' to restart";
                restart = true;
                break;
            }
        }
    }
    //void CheckObjectives()
    //{
    //    switch (levelNumber)
    //    {
    //        case 1:
    //            if (score >= 500)
    //                StartCoroutine(LoadNewLevel(levelNumber + 1));
    //            break;
    //        case 2:
    //            if (score >= 750)
    //                StartCoroutine(LoadNewLevel(levelNumber + 1));
    //            break;
    //        case 3:
    //            break;
    //        default:
    //            break;
    //    }
    //}
    //IEnumerator LoadNewLevel(int newLevelNumber)
    //{
    //    gameOverText.text = "Victory!!!";
    //    yield return new WaitForSeconds(5);
    //    Application.LoadLevel(newLevelNumber);
    //}
    //IEnumerator BossStates()
    //{
    //    yield return new WaitForSeconds(startWait);
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(waveWait);
    //        if (gameOver)
    //        {
    //            restartText.text = "Press 'R' to restart";
    //            restart = true;
    //            break;
    //        }
    //        if (victory)
    //        {
    //            restartText.text = "You Win";
    //            gameOverText.text = "Victory\nYour Score : " + score;
    //            break;
    //        }

    //    }
    //}
    public void AddScore(int scoreValue)
    {
        score.score += scoreValue;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score : " + score.score;
    }

    public void GameOver()
    {
        gameOver = true;
        gameOverText.text = "Game Over";
    }
}
public class Score
{
    public int score { get; set; }
    public Score()
    {
        score = 0;
    }
}
public class DestructableModel
{
    GameObject destructablePrefab;
    GameObject explosionPrefab;
    [Range(0,100)] 
    public int? spawnRate;
    public int hits;
    public int size;
    

}
