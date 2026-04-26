using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class BoxPacking : MonoBehaviour
{
    [Header("box components")]
    [SerializeField] Transform pizzaLocation;
    [Header("קריינות אריזה")]
    [Tooltip("עיכוב בשניות לפני הקריינות")]
    public float voiceDelay = 2f;
    public UnityEvent OnPacking;
    public UnityEvent OnPackingFinished;

    void OnTriggerEnter(Collider other)
    {
        Pizza pizza;
        other.gameObject.TryGetComponent(out pizza);
        if (pizza != null)
        {
            StartCoroutine(PackWithDelay(pizza));
        }
    }

    IEnumerator PackWithDelay(Pizza pizza)
    {
        // המתן 2 שניות
        yield return new WaitForSeconds(voiceDelay);

        // הפעל קריינות אריזה
        //FindFirstObjectByType<OnboardingManager>()?.OnboardingActionCompleted("packing");
        OnPackingFinished.Invoke();

        // הודע שהפיצה נמסרה
        if (pizza.Pack(pizzaLocation))
            print("Pizza delivered successfully");
        //FindFirstObjectByType<PizzaOrder>()?.PizzaDelivered(pizza);

    }
}