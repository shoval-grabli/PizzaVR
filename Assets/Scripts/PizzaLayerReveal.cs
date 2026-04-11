using UnityEngine;

public class PizzaLayerReveal : MonoBehaviour
{
    public GameObject layerToReveal;
    public string requiredTag;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(requiredTag))
        {
            PizzaOrder order = FindFirstObjectByType<PizzaOrder>();
            if (order == null) return;

            // בדוק אם זו התוספת הנכונה בתור
            if (!order.IsNextTopping(requiredTag))
            {
                // לא בתור — חזרה למקום
                order.ReturnIngredientToOrigin(requiredTag, collision.gameObject);
                return;
            }

            // תוספת נכונה ובתור — חשוף את השכבה
            layerToReveal.SetActive(true);
            order.ToppingAdded(requiredTag, collision.gameObject);
            collision.gameObject.SetActive(false);

            // הודע לאונבורדינג
            FindFirstObjectByType<OnboardingManager>()?.OnboardingActionCompleted(requiredTag);
        }
        
    }
    
    
}