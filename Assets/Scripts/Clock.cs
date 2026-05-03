using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour {

    public float secondsPerRound = 60f;
    public int totalRounds = 10;
    public bool isRunning = true;
    public float clockSpeed = 1.0f;

    [Header("מחוגים")]
    public GameObject pointerSeconds;
    public GameObject pointerMinutes;
    public GameObject pointerHours;

    [Header("UI - ספירת סיבובים")]
    public TMP_Text roundsText;

    private float timeRemaining;
    private int roundsLeft;

    void Start() 
    {
        timeRemaining = secondsPerRound * totalRounds;
        roundsLeft = totalRounds;
        UpdateRoundsUI();
    }

    void Update() 
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime * clockSpeed;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;
            OnTimerEnd();
            return;
        }

        roundsLeft = Mathf.CeilToInt(timeRemaining / secondsPerRound);
        UpdateRoundsUI();

        float secondsInCurrentRound = timeRemaining % secondsPerRound;
        float rotationSeconds = (360.0f / secondsPerRound) * secondsInCurrentRound;
        float rotationMinutes = rotationSeconds;
        float rotationHours   = (360.0f / totalRounds) * (totalRounds - roundsLeft);

        if (pointerSeconds != null)
            pointerSeconds.transform.localEulerAngles = new Vector3(0f, 0f, -rotationSeconds);
        if (pointerMinutes != null)
            pointerMinutes.transform.localEulerAngles = new Vector3(0f, 0f, -rotationMinutes);
        if (pointerHours != null)
            pointerHours.transform.localEulerAngles   = new Vector3(0f, 0f, -rotationHours);
    }

    void UpdateRoundsUI()
    {
        if (roundsText != null)
            roundsText.text = roundsLeft.ToString();
    }

    void OnTimerEnd()
    {
        Debug.Log("נגמרו כל הסיבובים!");
    }

    public float GetTimeRemaining() => timeRemaining;
}