using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class BoxPacking : MonoBehaviour
{
    [Header("קריינות אריזה")]
    [Tooltip("עיכוב בשניות לפני הקריינות")]
    public float voiceDelay = 2f;
    public UnityEvent OnPacking;
    public UnityEvent OnPackingFinished;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CookedDough"))
        {
            StartCoroutine(PackWithDelay(other.gameObject));
        }
    }

    IEnumerator PackWithDelay(GameObject pizza)
    {
        // המתן 2 שניות
        yield return new WaitForSeconds(voiceDelay);

        // הפעל קריינות אריזה
        //FindFirstObjectByType<OnboardingManager>()?.OnboardingActionCompleted("packing");
        OnPackingFinished.Invoke();

        // הודע שהפיצה נמסרה
        FindFirstObjectByType<PizzaOrder>()?.PizzaDelivered(pizza);
    }
}