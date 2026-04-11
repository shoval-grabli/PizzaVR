using UnityEngine;
using System.Collections;

public class OvenCooking : MonoBehaviour
{
    [Header("בישול")]
    public float cookingTime = 3f;
    public Color cookedColor = new Color(1f, 0.5f, 0f);
    public string pizzaTag = "Dough";

    [Header("אפקט תנור — זוהר אש")]
    public Renderer ovenInteriorRenderer;
    public Color fireColorA = new Color(1f, 0.3f, 0f, 1f);
    public Color fireColorB = new Color(1f, 0.8f, 0f, 1f);
    public float fireFlickerSpeed = 4f;

    [Header("אפקט אש — Particle System")]
    [Tooltip("גרור לכאן את ה-Particle System של האש")]
    public ParticleSystem fireParticles;

    private Color ovenOriginalColor;
    private Coroutine fireCoroutine;
    private bool isCooking = false;

    void Start()
    {
        if (ovenInteriorRenderer != null)
            ovenOriginalColor = ovenInteriorRenderer.materials[1].color;

        // וודא שה-Particle System כבוי בהתחלה
        if (fireParticles != null)
            fireParticles.Stop();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(pizzaTag) && !isCooking)
        {
            isCooking = true;
            StartCoroutine(CookPizza(other.gameObject));
        }
    }

    IEnumerator CookPizza(GameObject pizza)
    {
        // הפעל אפקט אש
        fireCoroutine = StartCoroutine(FireEffect());

        // הפעל Particle System
        if (fireParticles != null)
            fireParticles.Play();

        // המתן לסיום הבישול
        yield return new WaitForSeconds(cookingTime);

        // עצור אפקט אש
        StopFireEffect();

        // עצור Particle System
        if (fireParticles != null)
            fireParticles.Stop();

        // שנה צבע פיצה לכתום (אפויה)
        Renderer r = pizza.GetComponent<Renderer>();
        if (r != null)
            r.material.color = cookedColor;

        pizza.tag = "CookedDough";
        isCooking = false;

        FindFirstObjectByType<PizzaOrder>()?.PizzaCooked();
        FindFirstObjectByType<OnboardingManager>()?.OnboardingActionCompleted("oven");
    }

    IEnumerator FireEffect()
    {
        if (ovenInteriorRenderer == null) yield break;

        while (true)
        {
            float t = Mathf.PingPong(Time.time * fireFlickerSpeed, 1f);
            float noise = 1f + Random.Range(-0.1f, 0.1f);
            Color fireColor = Color.Lerp(fireColorA, fireColorB, t) * noise;
            fireColor.a = 1f;

            Material[] mats = ovenInteriorRenderer.materials;
            mats[1].color = fireColor;
            ovenInteriorRenderer.materials = mats;

            yield return null;
        }
    }

    void StopFireEffect()
    {
        if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
            fireCoroutine = null;
        }

        if (ovenInteriorRenderer != null)
        {
            Material[] mats = ovenInteriorRenderer.materials;
            mats[1].color = ovenOriginalColor;
            ovenInteriorRenderer.materials = mats;
        }
    }
}