using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameContoller : MonoBehaviour
{

    public GameObject[] roidFieldRocks;
    public GameObject[] enemies;
    public Vector3 spawnValues;
    public Text scoreText;
    public Text restartText;
    public Text gameOverText;
    public bool bossScene;

    public int hazardCount;
    public float spawnWait;
    public float waveWait;
    public float startWait;
    public int score;
    public bool menuOpen;
    public bool isSpawningLevel;
    public int levelNumber;

    public int roidNumber;
    public int maxRoidSize;

    private bool restart;
    private bool gameOver;
    public bool victory;
    private MainMenuController mainMenu;

    // Use this for initialization
    void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        mainMenu = canvas.GetComponentInChildren<MainMenuController>();
        menuOpen = mainMenu.mainMenu.activeSelf;
        victory = false;
        restart = false;
        gameOver = false;
        restartText.text = "";
        gameOverText.text = "";
        CreateRoidField(roidNumber, maxRoidSize);
        if (!bossScene)
            //StartCoroutine(SpawnWaves());
            //else
            //    StartCoroutine(BossStates());
            score = 0;
        StartCoroutine(CheckForDead());
        UpdateScore();
        levelNumber = Application.loadedLevel;
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
                Application.LoadLevel(Application.loadedLevel);
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
        // bool to keep player safe during spawning
        isSpawningLevel = true;
        // loop for the number of asteroids to be made
        for (int i = 0; i < roidNumber; i++)
        {
            // pull out the model collider from an array
            GameObject roid = roidFieldRocks[Random.Range(0, roidFieldRocks.Length)];
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
        score += scoreValue;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score : " + score;
    }

    public void GameOver()
    {
        gameOver = true;
        gameOverText.text = "Game Over";
    }
}
