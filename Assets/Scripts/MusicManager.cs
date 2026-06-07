using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;
    public AudioClip backGroundMusic;
    [SerializeField] private Slider musicSlider;
    private void Awake()
    {
		if (instance == null)
        {
			instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject); 
        }
		else if(instance != this)
		{
            Destroy (gameObject);
        }

    }
    void Start()
    {
        if (backGroundMusic != null)
        {
            PlayBackgroundMusic(false, backGroundMusic);
        }
        musicSlider.onValueChanged.AddListener(delegate { SetVolume(musicSlider.value); });
        
    }
    public static void SetVolume(float volume)
    {
        instance.audioSource.volume = volume;
    }

    public void PlayBackgroundMusic(bool resetSong, AudioClip audioClip = null)
    {
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }
        if (audioSource.clip != null)
        {
            if (resetSong)
            {
                audioSource.Stop();
            }
            audioSource.Play();
        }
    }
}
