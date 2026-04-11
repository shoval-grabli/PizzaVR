using UnityEngine;
using System.Collections;

public class PizzaOrder : MonoBehaviour
{
    [Header("ההזמנה")]
    public bool requiresSauce = false;
    public bool requiresCheese = false;
    public bool requiresOlives = false;
    public bool requiresOnion = false;
    public bool requiresMushroom = false;
    public bool requiresPepper = false;
    public float orderTime = 120f;
    public bool timerEnabled = true;
    
    [Header("מצב הפיצה הנוכחי")]
    public bool hasSauce = false;
    public bool hasCheese = false;
    public bool hasOlives = false;
    public bool hasOnion = false;
    public bool hasMushroom = false;
    public bool hasPepper = false;

    [Header("UI")]
    public TMPro.TextMeshProUGUI orderText;

    [Header("אונבורדינג")]
    public OnboardingManager onboardingManager;
    public UnityEngine.UI.Slider progressBar;

    [Header("הפיצה")]
    public GameObject pizzaObject;
    private Vector3 pizzaOrigin;
    private Color originalPizzaColor;

    private Vector3 sauceSphereOrigin;
    private Vector3 cheeseSphereOrigin;
    private Vector3 olivesSphereOrigin;
    private Vector3 onionSphereOrigin;
    private Vector3 mushroomSphereOrigin;
    private Vector3 pepperSphereOrigin;

    private float timeRemaining;
    private bool orderActive = false;
    private int currentToppingIndex = 0;
    private string[] toppingOrder = { "Sauce", "Cheese", "Onion", "Mushroom" };

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

        if (sauceSphere)    sauceSphereOrigin    = sauceSphere.transform.position;
        if (cheeseSphere)   cheeseSphereOrigin   = cheeseSphere.transform.position;
        if (olivesSphere)   olivesSphereOrigin   = olivesSphere.transform.position;
        if (onionSphere)    onionSphereOrigin     = onionSphere.transform.position;
        if (mushroomSphere) mushroomSphereOrigin  = mushroomSphere.transform.position;
        if (pepperSphere)   pepperSphereOrigin    = pepperSphere.transform.position;

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
            string display = timerEnabled ? "Your Order: " + Mathf.CeilToInt(timeRemaining) + "s\n\n" : "Your Order:\n\n";
            if (requiresSauce)    display += (hasSauce    ? "[V]" : "[ ]") + " Sauce\n";
            if (requiresCheese)   display += (hasCheese   ? "[V]" : "[ ]") + " Cheese\n";
            if (requiresOlives)   display += (hasOlives   ? "[V]" : "[ ]") + " Olives\n";
            if (requiresOnion)    display += (hasOnion    ? "[V]" : "[ ]") + " Onion\n";
            if (requiresMushroom) display += (hasMushroom ? "[V]" : "[ ]") + " Mushroom\n";
            if (requiresPepper)   display += (hasPepper   ? "[V]" : "[ ]") + " Pepper\n";
            orderText.text = display;
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
        requiresSauce = requiresCheese = requiresOlives = false;
        requiresOnion = requiresMushroom = requiresPepper = false;
        hasSauce = hasCheese = hasOlives = false;
        hasOnion = hasMushroom = hasPepper = false;

        bool[] toppings = new bool[6];
        int count = Random.Range(2, 4);

        while (count > 0)
        {
            int i = Random.Range(0, 6);
            if (!toppings[i]) { toppings[i] = true; count--; }
        }

        requiresSauce    = true;
        requiresCheese   = true;
        requiresOlives   = false;
        requiresOnion    = true;
        requiresMushroom = true;
        requiresPepper   = false;

        hasSauce = hasCheese = hasOlives = false;
        hasOnion = hasMushroom = hasPepper = false;
    }

    // ─────────────────────────────────────────────
    // Topping Logic
    // ─────────────────────────────────────────────

    public void ToppingAdded(string tag, GameObject ingredient)
    {
        if (currentToppingIndex >= toppingOrder.Length)
        {
            ReturnIngredientToOrigin(tag, ingredient);
            return;
        }

        string expectedTag = toppingOrder[currentToppingIndex];

        if (tag == expectedTag)
        {
            switch (tag)
            {
                case "Sauce":    hasSauce    = true; break;
                case "Cheese":   hasCheese   = true; break;
                case "Onion":    hasOnion    = true; break;
                case "Mushroom": hasMushroom = true; break;
            }
            currentToppingIndex++;
            PlaySuccessFeedback(); // ✅ משוב הצלחה לתוספת נכונה
            UpdateProgress();
            CheckOrderComplete();
        }
        else
        {
            ReturnIngredientToOrigin(tag, ingredient);
        }
    }

    public void ReturnIngredientToOrigin(string tag, GameObject ingredient)
    {
        switch (tag)
        {
            case "Sauce":    ReturnToOrigin(ingredient, sauceSphereOrigin);    break;
            case "Cheese":   ReturnToOrigin(ingredient, cheeseSphereOrigin);   break;
            case "Olives":   ReturnToOrigin(ingredient, olivesSphereOrigin);   break;
            case "Onion":    ReturnToOrigin(ingredient, onionSphereOrigin);    break;
            case "Mushroom": ReturnToOrigin(ingredient, mushroomSphereOrigin); break;
            case "Pepper":   ReturnToOrigin(ingredient, pepperSphereOrigin);   break;
        }
    }

    void ReturnToOrigin(GameObject ingredient, Vector3 origin)
    {
        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        ingredient.transform.position = origin;
        ingredient.transform.rotation = Quaternion.identity;
        if (rb != null) rb.isKinematic = false;
    }

    public bool IsToppingRequired(string tag)
    {
        switch (tag)
        {
            case "Sauce":    return requiresSauce;
            case "Cheese":   return requiresCheese;
            case "Olives":   return requiresOlives;
            case "Onion":    return requiresOnion;
            case "Mushroom": return requiresMushroom;
            case "Pepper":   return requiresPepper;
            default:         return false;
        }
    }

    // ─────────────────────────────────────────────
    // UI
    // ─────────────────────────────────────────────

    void UpdateUI()
    {
        if (orderText == null) return;

        string display = timerEnabled ? "Your Order: " + Mathf.CeilToInt(timeRemaining) + "s\n\n" : "Your Order:\n\n";
        if (requiresSauce)    display += (hasSauce    ? "[V]" : "[ ]") + " Sauce\n";
        if (requiresCheese)   display += (hasCheese   ? "[V]" : "[ ]") + " Cheese\n";
        if (requiresOlives)   display += (hasOlives   ? "[V]" : "[ ]") + " Olives\n";
        if (requiresOnion)    display += (hasOnion    ? "[V]" : "[ ]") + " Onion\n";
        if (requiresMushroom) display += (hasMushroom ? "[V]" : "[ ]") + " Mushroom\n";
        if (requiresPepper)   display += (hasPepper   ? "[V]" : "[ ]") + " Pepper\n";
        orderText.text = display;
    }

    void UpdateProgress()
    {
        if (progressBar == null) return;

        int totalSteps = 0;
        int completedSteps = 0;

        if (requiresSauce)    { totalSteps++; if (hasSauce)    completedSteps++; }
        if (requiresCheese)   { totalSteps++; if (hasCheese)   completedSteps++; }
        if (requiresOlives)   { totalSteps++; if (hasOlives)   completedSteps++; }
        if (requiresOnion)    { totalSteps++; if (hasOnion)    completedSteps++; }
        if (requiresMushroom) { totalSteps++; if (hasMushroom) completedSteps++; }
        if (requiresPepper)   { totalSteps++; if (hasPepper)   completedSteps++; }

        totalSteps += 2; // תנור + אריזה

        progressBar.value = totalSteps > 0 ? (float)completedSteps / totalSteps : 0f;
    }

    // ─────────────────────────────────────────────
    // Order Complete
    // ─────────────────────────────────────────────

    void CheckOrderComplete()
    {
        bool anyRequired = requiresSauce || requiresCheese || requiresOlives ||
                           requiresOnion || requiresMushroom || requiresPepper;
        if (!anyRequired) return;

        if (hasSauce == requiresSauce &&
            hasCheese == requiresCheese &&
            hasOlives == requiresOlives &&
            hasOnion == requiresOnion &&
            hasMushroom == requiresMushroom &&
            hasPepper == requiresPepper)
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

        if (sauceSphere)    { sauceSphere.SetActive(true);    sauceSphere.transform.position    = sauceSphereOrigin; }
        if (cheeseSphere)   { cheeseSphere.SetActive(true);   cheeseSphere.transform.position   = cheeseSphereOrigin; }
        if (olivesSphere)   { olivesSphere.SetActive(true);   olivesSphere.transform.position   = olivesSphereOrigin; }
        if (onionSphere)    { onionSphere.SetActive(true);    onionSphere.transform.position    = onionSphereOrigin; }
        if (mushroomSphere) { mushroomSphere.SetActive(true); mushroomSphere.transform.position = mushroomSphereOrigin; }
        if (pepperSphere)   { pepperSphere.SetActive(true);   pepperSphere.transform.position   = pepperSphereOrigin; }

        GenerateRandomOrder();
        timeRemaining = orderTime;
        orderActive = true;
        UpdateUI();
        UpdateProgress();
        currentToppingIndex = 0;
    }

    public bool IsNextTopping(string tag)
    {
        if (currentToppingIndex >= toppingOrder.Length) return false;
        return toppingOrder[currentToppingIndex] == tag;
    }
}