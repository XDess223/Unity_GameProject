using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerManager playerManager;

    #region Component

    [Header("Game Component")]
    public int MaxLives = 3;
    public int UsedLives = 0;
    public bool confirmExit = false;
    private int liveCountText;
    public bool isGameOver = false;
    public bool finishedGame = false;
    [SerializeField] private GameObject playerUI;
    [Header("Player Stats")]
    public float playerCurrentHealth = 100f;
    public float playerMaxHealth = 100f;
    [Header("UI Component")]
    public GameObject loseScreen;
    public GameObject confirmExitScreen;
    public GameObject winScreen;
    public GameObject buffDisplayPanel;
    public TMP_Text buffDurationText;
    public GameObject liveScreen;
    public TMP_Text liveText;

    [Header("Door Positions")]
    private Dictionary<int, Vector3> doorPositions = new Dictionary<int, Vector3>();
    public int previousSceneIndex = 0;

    #endregion

    [Header("Item Collection")]
    private HashSet<string> collectedItems = new HashSet<string>();
    public int playerEssences = 0;

    public void MarkItemAsCollected(string itemId)
    {
        if (!string.IsNullOrEmpty(itemId))
        {
            collectedItems.Add(itemId);
            Debug.Log($"Item collected and saved: {itemId}");
        }
    }

    // Method to check if an item has been collected
    public bool IsItemCollected(string itemId)
    {
        return !string.IsNullOrEmpty(itemId) && collectedItems.Contains(itemId);
    }

    // Method to reset all collected items (for game restart)
    public void ResetCollectedItems()
    {
        collectedItems.Clear();
        playerEssences = 0;
    }

    private void Awake()
    {
		if (instance == null)
        {
			instance = this;
            DontDestroyOnLoad(gameObject); 
        }
		else if(instance != this)
		{
            Destroy (gameObject);
        }

        SetupReferences();        

    }
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (isGameOver)
        {
            if (loseScreen != null)
                loseScreen.SetActive(true);
            if (playerUI != null)
                playerUI.SetActive(false);
            if (buffDisplayPanel != null)
                buffDisplayPanel.SetActive(false);
            if (playerManager != null)
                playerManager.enabled = false;
            if (liveScreen != null)
                liveScreen.SetActive(false);
        }
        else if (finishedGame)
        {
            if (winScreen != null)
                winScreen.SetActive(true);
            if (buffDisplayPanel != null)
                buffDisplayPanel.SetActive(false);
            if (playerManager != null)
                playerManager.enabled = false;
            if (liveScreen != null)
                liveScreen.SetActive(false);
            Debug.Log("You Win!");
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            confirmExit = true;
            if (confirmExitScreen != null)
                confirmExitScreen.SetActive(true);
            if (playerUI != null)
                playerUI.SetActive(false);
            if (buffDisplayPanel != null)
                buffDisplayPanel.SetActive(false);
            if (playerManager != null)
                playerManager.enabled = false;
            if (liveScreen != null)
                liveScreen.SetActive(false);
            Debug.Log("Press Esc");
        }
        else if (confirmExit == false)
        {
            if (loseScreen != null)
                loseScreen.SetActive(false);
            if (confirmExitScreen != null)
                confirmExitScreen.SetActive(false);
            if (winScreen != null)
                winScreen.SetActive(false);
        }
        else
        {
            SetLivesText();
        }
        
    }

    public int UpdateUsedLives()
    {
        UsedLives += 1;
        SetLivesText();
        if (UsedLives >= MaxLives)
        {
            isGameOver = true;
            if (playerManager != null)
                playerManager.enabled = false;
            if (liveScreen != null)
                liveScreen.SetActive(false);
            if (playerUI != null)
                playerUI.SetActive(false);
            if (buffDisplayPanel != null)
                buffDisplayPanel.SetActive(false);
                
            LoseGame();
        }

        return UsedLives;
    }

    private void SetLivesText()
    {
        liveCountText = MaxLives - UsedLives;
        liveText.text = "Remain lives: " + liveCountText.ToString();
    }

    public void CheckWinGame()
    {        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevels = SceneManager.sceneCountInBuildSettings;

        if (currentSceneIndex + 1 < maxLevels)
        {
            NextLevel();
        }
        else
        {
            finishedGame = true;
            // winScreen.SetActive(true);
        }
    }

    private void LoseGame()
    {
        loseScreen.SetActive(true);
    }

    public void RestartGame()
    {
        Debug.Log("Retry");
        UsedLives = 0;
        isGameOver = false;
        confirmExit = false;
        finishedGame = false;
        playerCurrentHealth = playerMaxHealth;
        ResetCollectedItems();
        doorPositions.Clear();

        SceneManager.LoadScene(1);
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReturnGame()
    {
        Debug.Log("Return to Game");
        confirmExit = false;
        playerUI.SetActive(true);
        playerManager.enabled = true;
        liveScreen.SetActive(true);
        confirmExitScreen.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void PreviousLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupReferences();
        isGameOver = false;
    }
    

    // Find all necessary references in the current scene
    void SetupReferences()
    {
        // Find player manager in the current scene
        playerManager = FindAnyObjectByType<PlayerManager>();
        
        if (playerManager != null)
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            
            if (previousSceneIndex > currentScene && doorPositions.ContainsKey(currentScene))
            {
                playerManager.transform.position = doorPositions[currentScene];
                playerManager.respawnPoint = doorPositions[currentScene];
                Debug.Log($"Returning to previous level: Player spawned at saved position: {doorPositions[currentScene]} for scene {currentScene}");
            }
            else
            {

                playerManager.respawnPoint = playerManager.transform.position;
                Debug.Log($"Using default spawn position in scene {currentScene}");
            }
        }
        
        // Find UI elements by tag
        GameObject loseScreenObj = GameObject.FindWithTag("LoseScreen");
        if (loseScreenObj != null)
        {
            loseScreen = loseScreenObj;
        }
        
        GameObject confirmExitObj = GameObject.FindWithTag("ExitScreen");
        if (confirmExitObj != null)
        {
            confirmExitScreen = confirmExitObj;
        }
        
        GameObject winObj = GameObject.FindWithTag("WinScreen");
        if (winObj != null)
        {
            winScreen = winObj;
        }

        GameObject liveScreenObj = GameObject.FindWithTag("LivesScreen");
        if (liveScreenObj != null)
        {
            liveScreen = liveScreenObj;

            TMP_Text[] textComponents = liveScreenObj.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text text in textComponents)
            {
                if (text.gameObject.CompareTag("LivesText"))
                {
                    liveText = text;
                    break;
                }
            }
            
            // If we found both UI elements, set the correct state based on player's boost status
            if (buffDurationText != null && playerManager.attackBoostActive)
            {
                buffDisplayPanel.SetActive(playerManager.attackBoostActive);
            }
        }
        
        GameObject playerUIObj = GameObject.FindWithTag("PlayerUI");
        if (playerUIObj != null)
        {
            playerUI = playerUIObj;
        }

        GameObject buffDisplayPanelObj = GameObject.FindWithTag("BuffDisplayPanel");
        if (buffDisplayPanelObj != null)
        {
            buffDisplayPanel = buffDisplayPanelObj;
            
            // Find the buff duration text
            TMP_Text[] textComponents = buffDisplayPanelObj.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text text in textComponents)
            {
                if (text.gameObject.CompareTag("BuffDurationText"))
                {
                    buffDurationText = text;
                    break;
                }
            }
            
            // If we found both UI elements, set the correct state based on player's boost status
            if (buffDurationText != null && playerManager.attackBoostActive)
            {
                buffDisplayPanel.SetActive(playerManager.attackBoostActive);
            }

        }
    
        SetLivesText();
    }

    public void SaveDoorPosition(int sceneIndex, Vector3 position)
    {
        if (doorPositions.ContainsKey(sceneIndex))
        {
            doorPositions[sceneIndex] = position;
        }
        else
        {
            doorPositions.Add(sceneIndex, position);
        }
        Debug.Log($"Saved door position for scene {sceneIndex}: {position}");
    }

    void OnDestroy()
    {
        // Unregister from the sceneLoaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
