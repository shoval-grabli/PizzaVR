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

    bool cooking;

    void OnTriggerEnter(Collider other)
    {
        if(cooking) return;
        Pizza pizza;
        other.gameObject.TryGetComponent(out pizza);
        if (pizza == null) return;
        if (!pizza.IsReadyForPacking()) return;
        
        cooking = true;
        other.gameObject.GetComponent<ReturnToOriginAuto>().Override = true;
        StartCoroutine(PackWithDelay(pizza));
        
    }

    IEnumerator PackWithDelay(Pizza pizza)
    {
        pizza.gameObject.transform.position = pizzaLocation.position;
        pizza.gameObject.transform.rotation = pizzaLocation.rotation;
        
        yield return new WaitForSeconds(voiceDelay);

        OnPackingFinished.Invoke();

        // הודע שהפיצה נמסרה
        if (pizza.IsReadyForPacking())
        {
            print("Pizza delivered successfully");
            pizza.Pack(pizzaLocation);
        }
        cooking = false;

    }
}