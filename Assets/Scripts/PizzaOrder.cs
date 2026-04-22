using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class PizzaOrder : MonoBehaviour
{
    [Header("ההזמנה")]
    public float orderTime = 120f;
    public bool timerEnabled = true;
    [SerializeField] List<PizzaIngredient> requiredIngredients;
    public List<PizzaIngredient> RequiredIngredients {  get { return requiredIngredients; }}

    [Header("UI")]
    public TMPro.TextMeshProUGUI orderText;

    [Header("אונבורדינג")]
    public OnboardingManager onboardingManager;
    public UnityEngine.UI.Slider progressBar;

    [Header("הפיצה")]
    public Pizza pizzaObject;
    private Vector3 pizzaOrigin;
    private Color originalPizzaColor;

    private float timeRemaining;
    private bool orderActive = false;
    private int currentToppingIndex = 0;
    private PizzaIngredient[] toppingOrder = { PizzaIngredient.Sauce, PizzaIngredient.Cheese, PizzaIngredient.Onion, PizzaIngredient.Mushroom };

    public bool AllRequirementsFulfilled { get => requiredIngredients.TrueForAll(ing => pizzaObject.RevealedLayers.Exists(layer => layer == ing)); }

    [Header("אובייקטי המרכיבים")]
    public GameObject sauceSphere;
    public GameObject cheeseSphere;
    public GameObject olivesSphere;
    public GameObject onionSphere;
    public GameObject mushroomSphere;
    public GameObject pepperSphere;

    // ─────────────────────────────────────────────
    // משוב הצלחה
    // ─────────────────────────────────────────────
    [Header("משוב הצלחה")]
    [Tooltip("גרור לכאן את קבצי האודיו: Excellent, Good job, great")]
    public AudioClip[] successClips;

    private AudioSource audioSource;

    // ─────────────────────────────────────────────
    // Start
    // ─────────────────────────────────────────────

    void Start()
    {
        // הגדר AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;


        if (pizzaObject)
        {
            pizzaOrigin = pizzaObject.transform.position;
            Renderer r = pizzaObject.GetComponent<Renderer>();
            if (r) originalPizzaColor = r.material.color;
        }

        GenerateRandomOrder();
        timeRemaining = orderTime;
        orderActive = true;
        UpdateUI();
        UpdateProgress();
        
        var grab = pizzaObject.GetComponent
            <UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null) grab.enabled = false;
    }

    // ─────────────────────────────────────────────
    // משוב הצלחה — פונקציות
    // ─────────────────────────────────────────────

    void PlaySuccessFeedback()
    {
        // נגן קריינות חיובית אקראית
        if (successClips != null && successClips.Length > 0)
        {
            AudioClip clip = successClips[Random.Range(0, successClips.Length)];
            if (audioSource != null && clip != null)
                audioSource.PlayOneShot(clip);
        }

        // אפקט ויזואלי עדין — זוהר ירוק קצר על הפיצה
        if (pizzaObject != null)
            StartCoroutine(FlashSuccess());
    }

    IEnumerator FlashSuccess()
    {
        Renderer r = pizzaObject.GetComponent<Renderer>();
        if (r == null) yield break;

        Color original = r.material.color;
        r.material.color = new Color(0.2f, 1f, 0.2f, 1f); // ירוק
        yield return new WaitForSeconds(0.5f);
        r.material.color = original;
    }

    // ─────────────────────────────────────────────
    // Update
    // ─────────────────────────────────────────────

    void Update()
    {
        if (!orderActive) return;

        if (timerEnabled) timeRemaining -= Time.deltaTime;

        if (orderText != null)
        {
            UpdateUI();
        }

        if (timeRemaining <= 0)
        {
            orderActive = false;
            Debug.Log("Time is up!");
            if (orderText != null)
                orderText.text = "Time is up!";
            Invoke("StartNewRound", 2f);
        }
    }

    // ─────────────────────────────────────────────
    // Order Generation
    // ─────────────────────────────────────────────

    void GenerateRandomOrder()
    {

        var rng = new System.Random();

        for (int i = requiredIngredients.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (requiredIngredients[i], requiredIngredients[j]) = (requiredIngredients[j], requiredIngredients[i]);
        }
    }


    // ─────────────────────────────────────────────
    // Topping Logic
    // ─────────────────────────────────────────────

    public void ToppingAdded(PizzaIngredient tag)
    {
        if (currentToppingIndex >= toppingOrder.Length)
        {
            return;
        }

        PizzaIngredient expectedTag = toppingOrder[currentToppingIndex];

        if (requiredIngredients.Exists(ing => ing == tag))
        {
            currentToppingIndex++;
            PlaySuccessFeedback(); // ✅ משוב הצלחה לתוספת נכונה
            UpdateProgress();
            CheckOrderComplete();
        }
        
    }



    public bool IsToppingRequired(PizzaIngredient tag)
    {
        return requiredIngredients.Exists(i  => i == tag);
    }

    // ─────────────────────────────────────────────
    // UI
    // ─────────────────────────────────────────────

    void UpdateUI()
    {
        string display = timerEnabled ? "Your Order: " + Mathf.CeilToInt(timeRemaining) + "s\n\n" : "Your Order:\n\n";
        for (int i = 0; i < requiredIngredients.Count; i++)
        {
            display += (pizzaObject.RevealedLayers.Exists(revealed => revealed == requiredIngredients[i]) ? "[V]" : "[ ]") + requiredIngredients[i].ToString() + ".\n";
        }
        orderText.text = display;
    }

    void UpdateProgress()
    {
        if (progressBar == null) return;

        int totalSteps = 0;
        int completedSteps = 0;

        totalSteps = requiredIngredients.Count;
        completedSteps = pizzaObject.RevealedLayers.Count;

        totalSteps += 2; // תנור + אריזה

        progressBar.value = totalSteps > 0 ? (float)completedSteps / totalSteps : 0f;
    }

    // ─────────────────────────────────────────────
    // Order Complete
    // ─────────────────────────────────────────────

    void CheckOrderComplete()
    {
        bool anyRequired = requiredIngredients.Count > 0;
        if (!anyRequired) return;

        if (AllRequirementsFulfilled)
        {
            orderActive = false;
            Debug.Log("Toppings done! Put in oven.");
            if (orderText != null)
                orderText.text = "Now put in oven!";

            var grab = pizzaObject.GetComponent
                <UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grab != null) grab.enabled = true;
            UpdateProgress();
        }
    }

    public void PizzaCooked()
    {
        PlaySuccessFeedback(); // ✅ משוב הצלחה — פיצה נאפתה
        Debug.Log("Pizza cooked! Now pack it.");
        if (orderText != null)
            orderText.text = "Now pack it!";
        if (progressBar != null) progressBar.value = 0.8f;
    }

    public void PizzaDelivered(GameObject pizza)
    {
        
        Debug.Log("Pizza complete!");
        if (orderText != null)
            orderText.text = "";

        orderActive = false;

        if (onboardingManager != null)
            onboardingManager.ShowFinalStep();

        if (progressBar != null) progressBar.value = 1f;
    }

    // ─────────────────────────────────────────────
    // New Round
    // ─────────────────────────────────────────────

    void StartNewRound()
    {
        if (pizzaObject)
        {
            pizzaObject.transform.position = pizzaOrigin;
            pizzaObject.tag = "Dough";
            Renderer r = pizzaObject.GetComponent<Renderer>();
            if (r) r.material.color = originalPizzaColor;

            foreach (Transform child in pizzaObject.transform)
                child.gameObject.SetActive(false);
        }


        GenerateRandomOrder();
        timeRemaining = orderTime;
        orderActive = true;
        UpdateUI();
        UpdateProgress();
        currentToppingIndex = 0;
    }

    public bool IsNextTopping(PizzaIngredient tag)
    {
        if (currentToppingIndex >= toppingOrder.Length) return false;
        return toppingOrder[currentToppingIndex] == tag;
    }
}