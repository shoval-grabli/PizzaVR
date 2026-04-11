using UnityEngine;
using System.Collections;

public class BoxPacking : MonoBehaviour
{
    [Header("קריינות אריזה")]
    [Tooltip("עיכוב בשניות לפני הקריינות")]
    public float voiceDelay = 2f;

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
        FindFirstObjectByType<OnboardingManager>()?.OnboardingActionCompleted("packing");

        // הודע שהפיצה נמסרה
        FindFirstObjectByType<PizzaOrder>()?.PizzaDelivered(pizza);
    }
}