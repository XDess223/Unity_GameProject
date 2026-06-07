using UnityEngine;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    GameManager gameManager => GameManager.instance;
    public Button button;
    void Start()
    {
        if (gameManager.confirmExitScreen != null)
        {
            // Find the Cancel button
            Button cancelButton = null;
            Button exitButton = null;
            Button[] buttons = gameManager.confirmExitScreen.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                if (button.gameObject.CompareTag("Cancel_Button"))
                {
                    cancelButton = button;
                }
                else if (button.gameObject.CompareTag("Exit_Button_Exit"))
                {
                    exitButton = button;
                }
            }
            
            // Setup Cancel button listener
            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
                cancelButton.onClick.AddListener(gameManager.ReturnGame);
                Debug.Log("Exit confirmation Cancel button listener set up");
            }

            if (exitButton != null)
            {
                exitButton.onClick.RemoveAllListeners();
                exitButton.onClick.AddListener(gameManager.ExitGame);
                Debug.Log("Exit confirmation Exit button listener set up");
            }
        }
        
        if (gameManager.loseScreen != null)
        {
            Button retryButton = null;
            Button exitButton = null;
            Button[] buttons = gameManager.loseScreen.GetComponentsInChildren<Button>();
            
            foreach (Button button in buttons)
            {
                if (button.gameObject.CompareTag("Retry_Button_Lose"))
                {
                    retryButton = button;
                }
                else if (button.gameObject.CompareTag("Exit_Button_Lose"))
                {
                    exitButton = button;
                }
            }
            
            if (retryButton != null)
            {
                retryButton.onClick.RemoveAllListeners();
                retryButton.onClick.AddListener(gameManager.RestartGame);
                Debug.Log("Lose screen Retry button listener set up");
            }
            
            if (exitButton != null)
            {
                exitButton.onClick.RemoveAllListeners();
                exitButton.onClick.AddListener(gameManager.ExitGame);
                Debug.Log("Lose screen Exit button listener set up");
            }

            
        }

        if (gameManager.winScreen != null)
        {
            Button retryButton = null;
            Button exitButton = null;
            Button[] buttons = gameManager.winScreen.GetComponentsInChildren<Button>();
            
            foreach (Button button in buttons)
            {
                if (button.gameObject.CompareTag("Retry_Button_Win"))
                {
                    retryButton = button;
                }
                else if (button.gameObject.CompareTag("Exit_Button_Win"))
                {
                    exitButton = button;
                }
            }
            
            if (retryButton != null)
            {
                retryButton.onClick.RemoveAllListeners();
                retryButton.onClick.AddListener(gameManager.RestartGame);
                Debug.Log("Win screen Retry button listener set up");
            }
            
            if (exitButton != null)
            {
                exitButton.onClick.RemoveAllListeners();
                exitButton.onClick.AddListener(gameManager.ExitGame);
                Debug.Log("Win screen Exit button listener set up");
            }
        }

        
    }

    private void Oestroy()
    {
        
    }
}
