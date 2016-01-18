using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject levelMenu;
    public GameObject thisMenu;
    public GameObject border;
    public GameContoller gc;

    void OnAwake() {
        if (!levelMenu)
        {
            levelMenu = null; 
        }
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnBackToMain()
    {
        thisMenu.SetActive(false);
        mainMenu.SetActive(true);
        if (gc != null)
            gc.UpdateMenuStatus();
    }
    public void OnOpenOptions()
    {
        thisMenu = optionsMenu;
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void OnOpenLevelSelect ()
    {
        thisMenu = levelMenu;
        levelMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void OnStartGame()
    {
        Application.LoadLevel(1);
    }
    public void LoadLevel(int levelToLoad)
    {
        Time.timeScale = 1;
         Application.LoadLevel(levelToLoad);
    }
    public void OnExitGame()
    {
        Application.Quit();
    }
    public void OnInGameMenuOpen()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        mainMenu.SetActive(true);
        border.SetActive(true);
        gc.UpdateMenuStatus();
    }
    public void OnInGameMenuClose()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        Time.timeScale = 1;
        mainMenu.SetActive(false);
        border.SetActive(false);
        gc.UpdateMenuStatus();
    }
}
