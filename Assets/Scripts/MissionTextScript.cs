using UnityEngine;
using System.Collections;
using TMPro;

public class MissionTextScript : MonoBehaviour
{
    [Header("Message Settings")]
    [SerializeField] private string introMessage = "Kill Enemies For Essence To Pass The Level";
    [SerializeField] private float displayDuration = 2.0f;
    [SerializeField] private int fontSize = 36;
    
    [Header("Animation")]
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float fadeOutTime = 0.5f;
    
    private TMP_Text messageText;
    private CanvasGroup canvasGroup;
    
    private void Awake()
    {
        // Create canvas if not present
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // Ensure it's visible above other UI
        }
        
        // Create canvas scaler if not present
        if (GetComponent<UnityEngine.UI.CanvasScaler>() == null)
        {
            UnityEngine.UI.CanvasScaler scaler = gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }
        
        // Create canvas group if not present
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
        }
        
        // Create text if not present
        messageText = GetComponentInChildren<TMP_Text>();
        if (messageText == null)
        {
            // Create a child GameObject for the text
            GameObject textObj = new GameObject("IntroMessageText");
            textObj.transform.SetParent(transform, false);
            
            // Add RectTransform and set it to fill
            RectTransform rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            // Add TextMeshPro component
            messageText = textObj.AddComponent<TMP_Text>();
            messageText.alignment = TextAlignmentOptions.Center;
            messageText.fontSize = fontSize;
            
        }
        
        // Set the message
        messageText.text = introMessage;
    }
    
    private void Start()
    {
        // Show the message at the start of the level
        StartCoroutine(ShowMessage());
    }
    
    private IEnumerator ShowMessage()
    {
        // Fade in
        float t = 0;
        while (t < fadeInTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeInTime);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        
        // Display for duration
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out
        t = 0;
        while (t < fadeOutTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeOutTime);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        
        // Optionally destroy after done
        Destroy(gameObject);
    }
}
