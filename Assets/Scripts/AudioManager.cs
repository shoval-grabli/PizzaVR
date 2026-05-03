using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sound Clips")]
    public AudioClip putInOvenSound;
    public AudioClip putInBoxSound;
    public AudioClip wrongAnswerSound;
    public AudioClip correctAnswerSound;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton - רק אחד קיים בסצנה
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPutInOven()
    {
        audioSource.PlayOneShot(putInOvenSound);
    }

    public void PlayPutInBox()
    {
        audioSource.PlayOneShot(putInBoxSound);
    }

    public void PlayWrongAnswer()
    {
        audioSource.PlayOneShot(wrongAnswerSound);
    }

    public void PlayCorrectAnswer()
    {
        audioSource.PlayOneShot(correctAnswerSound);
    }
}