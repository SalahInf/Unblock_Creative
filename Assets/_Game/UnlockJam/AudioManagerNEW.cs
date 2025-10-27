using UnityEngine;

public class AudioManagerNEW : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip sfxClicSource;
    [SerializeField] private AudioClip sfxSwipeSource;

    public static AudioManagerNEW instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayClick()
    {
        musicSource.clip = sfxClicSource;
        musicSource.Play();
    }

   public void PlaySwipe()
    {
        musicSource.clip = sfxSwipeSource;
        musicSource.Play();
    }

}
