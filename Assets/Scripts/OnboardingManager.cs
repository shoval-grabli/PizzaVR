using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// OnboardingManager - מנהל האונבורדינג של משחק הפיצרייה
/// מיועד לתלמידי חינוך מיוחד כיתות ז-ט, תפקוד קוגניטיבי בינוני-גבוה
/// 5 שלבים: ידיים VR → קריאת הזמנה → הוספת תוספות → תנור ואריזה → התחלה
/// </summary>
public class OnboardingManager : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Inspector Fields
    // ─────────────────────────────────────────────

    [Header("=== שלבי האונבורדינג ===")]
    public bool runOnStart = true;
    public string gameSceneName = "Level1Scene";

    [Header("=== UI ===")]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI subtitleText;
    public GameObject startButton;
    public GameObject skipButton;
    public GameObject replayButton;

    [Header("=== חצים והדגשות ===")]
    public GameObject arrowIndicator;
    public GameObject highlightHalo;

    [Header("=== מטרות שהחץ יצביע עליהן (לפי שלב) ===")]
    public Transform targetHands;
    public Transform targetOrderBoard;
    public Transform targetSauceSphere;
    public Transform targetPizza;
    public Transform targetOven;
    public Transform targetBox;

    [Header("=== אובייקטים לאפקט זוהר ===")]
    [Tooltip("לוח ההזמנה — יזהיר בשלב 2")]
    public Renderer glowOrderBoard;

    [Tooltip("כדור הרוטב — יזהיר בשלב 3")]
    public Renderer glowSauce;

    [Tooltip("התוספות — יזהירו בשלב 4")]
    public Renderer[] glowToppings;

    [Tooltip("התנור — יזהיר בשלב 5")]
    public Renderer glowOven;

    [Tooltip("הקופסה/מגש — יזהיר בשלב 6")]
    public Renderer glowBox;

    [Header("=== קריינות קולית ===")]
    public AudioClip voiceOpening;
    public AudioClip voiceHands;
    public AudioClip voiceOrderBoard;
    public AudioClip voiceSauce;
    public AudioClip voiceToppings;
    public AudioClip voiceOven;
    public AudioClip voicePacking;
    public AudioClip voiceFinish;
    public AudioClip voicePositiveFeedback;

    [Header("=== הגדרות טיימינג ===")]
    public float autoAdvanceDelay = 4f;
    public float textFadeDuration = 0.5f;

    [Header("=== הגדרות ויזואליות ===")]
    public Color highlightColor = new Color(1f, 0.92f, 0.016f, 0.8f);
    public Color successColor   = new Color(0.2f, 1f, 0.2f, 0.8f);
    public Color glowColor      = new Color(1f, 0.85f, 0f, 1f); // צהוב זוהר לאובייקטים
    public bool  pulsingHalo    = true;
    public float pulseSpeed     = 2f;

    // ─────────────────────────────────────────────
    // Private State
    // ─────────────────────────────────────────────

    private AudioSource audioSource;
    private int  currentStep          = 0;
    private bool onboardingActive     = false;
    private bool waitingForPlayerAction = false;

    // שמירת הצבעים המקוריים של האובייקטים
    private Color originalOrderBoardColor;
    private Color originalSauceColor;
    private Color[] originalToppingColors;
    private Color originalOvenColor;
    private Color originalBoxColor;

    // Coroutine פעיל של הזוהר
    private Coroutine activeGlowCoroutine;

    private struct OnboardingStep
    {
        public string    hebrewText;
        public string    subtitleHebrew;
        public Transform arrowTarget;
        public AudioClip voiceClip;
        public bool      requiresAction;
        public string    actionHint;
        public Renderer[] glowTargets; // אובייקטים שיזהירו בשלב זה
    }

    private OnboardingStep[] steps;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
    }

    void Start()
    {
        SaveOriginalColors();
        BuildSteps();
        SetupInitialUI();
        if (runOnStart) StartOnboarding();
    }

    void Update()
    {
        if (!onboardingActive) return;
        if (pulsingHalo && highlightHalo != null && highlightHalo.activeSelf)
        {
            float scale = 1f + 0.15f * Mathf.Sin(Time.time * pulseSpeed);
            highlightHalo.transform.localScale = Vector3.one * scale;
        }
    }

    // ─────────────────────────────────────────────
    // שמירת צבעים מקוריים
    // ─────────────────────────────────────────────

    void SaveOriginalColors()
    {
        if (glowOrderBoard != null) originalOrderBoardColor = glowOrderBoard.material.color;
        if (glowSauce      != null) originalSauceColor      = glowSauce.material.color;
        if (glowOven       != null) originalOvenColor       = glowOven.material.color;
        if (glowBox        != null) originalBoxColor        = glowBox.material.color;

        if (glowToppings != null)
        {
            originalToppingColors = new Color[glowToppings.Length];
            for (int i = 0; i < glowToppings.Length; i++)
                if (glowToppings[i] != null)
                    originalToppingColors[i] = glowToppings[i].material.color;
        }
    }

    // ─────────────────────────────────────────────
    // Build Steps
    // ─────────────────────────────────────────────

    void BuildSteps()
    {
        steps = new OnboardingStep[]
        {
            // שלב 0: פתיחה — אין זוהר
            new OnboardingStep
            {
                hebrewText     = "ברוכים הבאים לפיצרייה.\nכאן נכין פיצות לפי הזמנות של לקוחות.\nכדי ללמוד איך עובדים במטבח, נכין יחד את הפיצה הראשונה.\nעקבו אחרי ההוראות והכינו את הפיצה שלב אחרי שלב.",
                subtitleHebrew = "שלב 1 מתוך 8 — פתיחה",
                arrowTarget    = null,
                voiceClip      = voiceOpening,
                requiresAction = false,
                actionHint     = "",
                glowTargets    = null
            },

            // שלב 1: ידיים — אין זוהר על אובייקט
            new OnboardingStep
            {
                hebrewText     = "לפני שמתחילים, הסתכלו על הידיים שלכם.\nבעזרת הידיים תוכלו לקחת מצרכים, להכין את הפיצה ולהפעיל את התנור.\nנסו להזיז את הידיים ולהתרגל לשליטה.",
                subtitleHebrew = "שלב 2 מתוך 8 — הכרות עם הידיים",
                arrowTarget    = targetHands,
                voiceClip      = voiceHands,
                requiresAction = false,
                actionHint     = "",
                glowTargets    = null
            },

            // שלב 2: לוח ההזמנה — זוהר על לוח ההזמנה
            new OnboardingStep
            {
                hebrewText     = "עכשיו הסתכלו על לוח ההזמנות.\nכאן מופיעה ההזמנה של הלקוח.\nלפי ההזמנה נדע אילו תוספות צריך להוסיף לפיצה.\nבואו נכין את הפיצה הראשונה.",
                subtitleHebrew = "שלב 3 מתוך 8 — לוח ההזמנה",
                arrowTarget    = targetOrderBoard,
                voiceClip      = voiceOrderBoard,
                requiresAction = false,
                actionHint     = "",
                glowTargets    = glowOrderBoard != null ? new Renderer[]{ glowOrderBoard } : null
            },

            // שלב 3: הרוטב — זוהר על הרוטב
            new OnboardingStep
            {
                hebrewText     = "נתחיל עם הרוטב.\nקחו את הרוטב והוסיפו אותו על הפיצה.\nהרוטב הוא הבסיס לכל פיצה.",
                subtitleHebrew = "שלב 4 מתוך 8 — הרוטב",
                arrowTarget    = targetSauceSphere,
                voiceClip      = voiceSauce,
                requiresAction = true,
                actionHint     = "👆 קחו את הרוטב והניחו אותו על הפיצה",
                glowTargets    = glowSauce != null ? new Renderer[]{ glowSauce } : null
            },

            // שלב 4: התוספות — זוהר על כל התוספות
            new OnboardingStep
            {
                hebrewText     = "עכשיו הסתכלו שוב על ההזמנה של הלקוח.\nלפי ההזמנה, קחו את התוספות המתאימות והוסיפו אותן על הפיצה.\nשימו לב שאתם מוסיפים בדיוק את התוספות שמופיעות בהזמנה.",
                subtitleHebrew = "שלב 5 מתוך 8 — התוספות",
                arrowTarget    = targetPizza,
                voiceClip      = voiceToppings,
                requiresAction = true,
                actionHint     = "",
                glowTargets    = glowToppings
            },

            // שלב 5: התנור — זוהר על התנור
            new OnboardingStep
            {
                hebrewText     = "מעולה. הפיצה מוכנה לאפייה.\nקחו את הפיצה והכניסו אותה לתנור.\nהתנור יאפה את הפיצה עד שתהיה מוכנה.",
                subtitleHebrew = "שלב 6 מתוך 8 — התנור",
                arrowTarget    = targetOven,
                voiceClip      = voiceOven,
                requiresAction = true,
                actionHint     = "",
                glowTargets    = glowOven != null ? new Renderer[]{ glowOven } : null
            },

            // שלב 6: האריזה — זוהר על הקופסה
            new OnboardingStep
            {
                hebrewText     = "הפיצה מוכנה.\nהוציאו את הפיצה מהתנור והכניסו אותה לקופסה.\nכך הפיצה מוכנה למסירה ללקוח.",
                subtitleHebrew = "שלב 7 מתוך 8 — האריזה",
                arrowTarget    = targetBox,
                voiceClip      = voicePacking,
                requiresAction = true,
                actionHint     = "",
                glowTargets    = glowBox != null ? new Renderer[]{ glowBox } : null
            },

            // שלב 7: סיום — אין זוהר
            new OnboardingStep
            {
                hebrewText     = "מעולה.\nהרגע הכנתם את הפיצה הראשונה שלכם.\nעכשיו אתם מוכנים להתחיל להכין פיצות לפי הזמנות של לקוחות.\nבהצלחה במטבח!",
                subtitleHebrew = "שלב 8 מתוך 8 — סיום",
                arrowTarget    = null,
                voiceClip      = voiceFinish,
                requiresAction = false,
                actionHint     = "",
                glowTargets    = null
            }
        };
    }

    // ─────────────────────────────────────────────
    // Glow Effects
    // ─────────────────────────────────────────────

    void StartGlow(Renderer[] targets)
    {
        StopGlow(); // עצור זוהר קודם
        if (targets == null || targets.Length == 0) return;
        activeGlowCoroutine = StartCoroutine(GlowLoop(targets));
    }

    void StopGlow()
    {
        if (activeGlowCoroutine != null)
        {
            StopCoroutine(activeGlowCoroutine);
            activeGlowCoroutine = null;
        }
        // החזר צבעים מקוריים
        RestoreAllColors();
    }

    IEnumerator GlowLoop(Renderer[] targets)
    {
        while (true)
        {
            // הבהוב — עלייה לצהוב
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * pulseSpeed;
                SetGlowColor(targets, Color.Lerp(GetOriginalColor(targets[0]), glowColor, t));
                yield return null;
            }
            // ירידה לצבע מקורי
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * pulseSpeed;
                SetGlowColor(targets, Color.Lerp(glowColor, GetOriginalColor(targets[0]), t));
                yield return null;
            }
        }
    }

    void SetGlowColor(Renderer[] targets, Color color)
    {
        foreach (var r in targets)
            if (r != null) r.material.color = color;
    }

    Color GetOriginalColor(Renderer r)
    {
        if (r == glowOrderBoard) return originalOrderBoardColor;
        if (r == glowSauce)      return originalSauceColor;
        if (r == glowOven)       return originalOvenColor;
        if (r == glowBox)        return originalBoxColor;
        // תוספות
        if (glowToppings != null)
            for (int i = 0; i < glowToppings.Length; i++)
                if (glowToppings[i] == r && originalToppingColors != null && i < originalToppingColors.Length)
                    return originalToppingColors[i];
        return Color.white;
    }

    void RestoreAllColors()
    {
        if (glowOrderBoard != null) glowOrderBoard.material.color = originalOrderBoardColor;
        if (glowSauce      != null) glowSauce.material.color      = originalSauceColor;
        if (glowOven       != null) glowOven.material.color       = originalOvenColor;
        if (glowBox        != null) glowBox.material.color        = originalBoxColor;
        if (glowToppings != null && originalToppingColors != null)
            for (int i = 0; i < glowToppings.Length; i++)
                if (glowToppings[i] != null && i < originalToppingColors.Length)
                    glowToppings[i].material.color = originalToppingColors[i];
    }

    // ─────────────────────────────────────────────
    // Setup
    // ─────────────────────────────────────────────

    void SetupInitialUI()
    {
        if (startButton)    startButton.SetActive(false);
        if (replayButton)   replayButton.SetActive(false);
        if (skipButton)     skipButton.SetActive(false);
        if (arrowIndicator) arrowIndicator.SetActive(false);
        if (highlightHalo)  highlightHalo.SetActive(false);
        if (instructionText) instructionText.gameObject.SetActive(false);
        if (subtitleText)    subtitleText.gameObject.SetActive(false);
    }

    // ─────────────────────────────────────────────
    // Public API
    // ─────────────────────────────────────────────

    public void StartOnboarding()
    {
        currentStep = 0;
        onboardingActive = true;
        if (skipButton)  skipButton.SetActive(true);
        if (startButton) startButton.SetActive(false);
        if (replayButton) replayButton.SetActive(false);
        StartCoroutine(RunStep(currentStep));
    }

    public void SkipOnboarding()
    {
        StopAllCoroutines();
        StopGlow();
        FinishOnboarding();
    }

    public void ReplayOnboarding()
    {
        StopAllCoroutines();
        UnityEngine.SceneManagement.SceneManager
            .LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void BeginGame()
    {
        FinishOnboarding();
    }

    public void OnboardingActionCompleted(string actionTag)
    {
        if (!onboardingActive || !waitingForPlayerAction) return;

        PizzaOrder order = FindFirstObjectByType<PizzaOrder>();

        if (currentStep == 3 && actionTag == "Sauce")
        {
            waitingForPlayerAction = false;
            StartCoroutine(ShowSuccessAndAdvance());
            return;
        }

        if (currentStep == 4 && order != null)
        {
            if (order.hasCheese == order.requiresCheese &&
                order.hasOlives == order.requiresOlives &&
                order.hasOnion  == order.requiresOnion  &&
                order.hasMushroom == order.requiresMushroom &&
                order.hasPepper == order.requiresPepper)
            {
                waitingForPlayerAction = false;
                StartCoroutine(ShowSuccessAndAdvance());
            }
            return;
        }

        waitingForPlayerAction = false;
        StartCoroutine(ShowSuccessAndAdvance());
    }

    // ─────────────────────────────────────────────
    // Step Execution
    // ─────────────────────────────────────────────

    IEnumerator RunStep(int stepIndex)
    {
        if (stepIndex >= steps.Length)
        {
            FinishOnboarding();
            yield break;
        }

        OnboardingStep step = steps[stepIndex];

        yield return StartCoroutine(FadeInText(step.hebrewText, step.subtitleHebrew));

        PointArrowAt(step.arrowTarget);
        HighlightTarget(step.arrowTarget);

        // הפעל זוהר על האובייקטים הרלוונטיים לשלב זה
        StartGlow(step.glowTargets);

        PlayVoice(step.voiceClip);

        if (step.requiresAction)
        {
            waitingForPlayerAction = true;
            if (!string.IsNullOrEmpty(step.actionHint))
                AppendHintToText(step.actionHint);
            yield return new WaitUntil(() => !waitingForPlayerAction);
        }
        else
        {
            if (audioSource.clip != null)
                yield return new WaitUntil(() => !audioSource.isPlaying);

            yield return new WaitForSeconds(0.5f);

            if (stepIndex == steps.Length - 1)
            {
                StopGlow();
                ShowFinalButtons();
                yield break;
            }

            StopGlow(); // עצור זוהר לפני מעבר לשלב הבא
            AdvanceStep();
        }
    }

    void ShowFinalButtons()
    {
        if (arrowIndicator) arrowIndicator.SetActive(false);
        if (highlightHalo)  highlightHalo.SetActive(false);
        if (startButton)    startButton.SetActive(true);
        if (replayButton)   replayButton.SetActive(true);
        if (skipButton)     skipButton.SetActive(false);
    }

    void AdvanceStep()
    {
        currentStep++;
        if (currentStep >= steps.Length)
        {
            ShowFinalStep();
            return;
        }
        StartCoroutine(RunStep(currentStep));
    }

    IEnumerator ShowSuccessAndAdvance()
    {
        StopGlow();
        yield return StartCoroutine(ShowSuccessFeedback());
        yield return new WaitForSeconds(1.2f);
        AdvanceStep();
    }

    // ─────────────────────────────────────────────
    // Final Step
    // ─────────────────────────────────────────────

    public void ShowFinalStep()
    {
        PlayVoice(steps[steps.Length - 1].voiceClip);
        StartCoroutine(RunStep(steps.Length - 1));
    }

    void FinishOnboarding()
    {
        onboardingActive = false;
        StopGlow();
        if (arrowIndicator) arrowIndicator.SetActive(false);
        if (highlightHalo)  highlightHalo.SetActive(false);
        if (startButton)    startButton.SetActive(false);
        if (skipButton)     skipButton.SetActive(false);
        if (replayButton)   replayButton.SetActive(false);
        SetInstructionText("");
        if (subtitleText) subtitleText.text = "";
        Debug.Log("[Onboarding] האונבורדינג הסתיים — עובר ל: " + gameSceneName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    // ─────────────────────────────────────────────
    // Arrow & Highlight
    // ─────────────────────────────────────────────

    void PointArrowAt(Transform target)
    {
        if (arrowIndicator == null) return;
        if (target == null) { arrowIndicator.SetActive(false); return; }
        arrowIndicator.SetActive(true);
        arrowIndicator.transform.position = target.position + Vector3.up * 0.3f;
        Camera cam = Camera.main;
        if (cam != null) arrowIndicator.transform.LookAt(cam.transform);
    }

    void HighlightTarget(Transform target)
    {
        if (highlightHalo == null) return;
        if (target == null) { highlightHalo.SetActive(false); return; }
        highlightHalo.SetActive(true);
        highlightHalo.transform.position = target.position;
        Renderer r = highlightHalo.GetComponent<Renderer>();
        if (r != null) r.material.color = highlightColor;
    }

    // ─────────────────────────────────────────────
    // Audio
    // ─────────────────────────────────────────────

    void PlayVoice(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    // ─────────────────────────────────────────────
    // Visual Feedback
    // ─────────────────────────────────────────────

    IEnumerator ShowSuccessFeedback()
    {
        if (highlightHalo != null)
        {
            Renderer r = highlightHalo.GetComponent<Renderer>();
            if (r != null) r.material.color = successColor;
        }
        SetInstructionText("✓ כל הכבוד!\nמעולה!");
        PlayVoice(voicePositiveFeedback);
        yield return new WaitForSeconds(1.5f);
        if (highlightHalo != null)
        {
            Renderer r = highlightHalo.GetComponent<Renderer>();
            if (r != null) r.material.color = highlightColor;
        }
    }

    // ─────────────────────────────────────────────
    // UI Helpers
    // ─────────────────────────────────────────────

    void SetInstructionText(string text)
    {
        if (instructionText != null) instructionText.text = text;
    }

    void AppendHintToText(string hint)
    {
        if (instructionText != null) instructionText.text += "\n\n" + hint;
    }

    IEnumerator FadeInText(string mainText, string sub)
    {
        if (instructionText != null)
        {
            Color c = instructionText.color;
            float elapsed = 0f;
            while (elapsed < textFadeDuration)
            {
                elapsed += Time.deltaTime;
                c.a = 1f - (elapsed / textFadeDuration);
                instructionText.color = c;
                yield return null;
            }
            c.a = 0f;
            instructionText.color = c;
        }

        SetInstructionText(mainText);
        if (subtitleText != null) subtitleText.text = sub;

        if (instructionText != null)
        {
            Color c = instructionText.color;
            float elapsed = 0f;
            while (elapsed < textFadeDuration)
            {
                elapsed += Time.deltaTime;
                c.a = elapsed / textFadeDuration;
                instructionText.color = c;
                yield return null;
            }
            c.a = 1f;
            instructionText.color = c;
        }
    }

    // ─────────────────────────────────────────────
    // Gizmos
    // ─────────────────────────────────────────────

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Transform[] targets = { targetHands, targetOrderBoard, targetSauceSphere, targetPizza, targetOven, targetBox };
        string[]    labels  = { "ידיים", "הזמנה", "רוטב", "פיצה", "תנור", "קופסה" };
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
            {
                Gizmos.DrawWireSphere(targets[i].position, 0.1f);
                UnityEditor.Handles.Label(targets[i].position + Vector3.up * 0.2f, labels[i]);
            }
        }
    }
#endif
}