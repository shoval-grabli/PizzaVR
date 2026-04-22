using UnityEngine;
using UnityEngine.Events;

public class PizzaLayerReveal : MonoBehaviour
{
    [SerializeField] Pizza pizza;
    public GameObject layerToReveal;
    public PizzaIngredient layersIngredient;
    [SerializeField] string requiredTag;
    [SerializeField] UnityEvent OnToppingAdded;
    
    PizzaToppingMono ingredient;
    bool revealed = false;
    public bool Revealed { get { return revealed; } }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(requiredTag))
        {
            other.gameObject.TryGetComponent(out ingredient);
            
            if (ingredient == null) return;

            // בדוק אם זו התוספת הנכונה בתור
            if (!pizza.Order.IsNextTopping(ingredient.topping.toppingName))
            {
                // לא בתור — חזרה למקום
                ingredient.ReturnToOrigin();
                return;
            }

            // תוספת נכונה ובתור — חשוף את השכבה
            pizza.ActivateLayer(this);
            pizza.Order.ToppingAdded(ingredient.topping.toppingName);
            ingredient.ReturnToOrigin();
            ingredient = null;
            //collision.gameObject.SetActive(false);

            // הודע לאונבורדינג
            //FindFirstObjectByType<OnboardingManager>()?.OnboardingActionCompleted(ingredient.toppingName.ToString());
            OnToppingAdded.Invoke();
        }
        
    }
    public void Reveal()
    {
        if(revealed) return;
        print("Revealed");
        revealed = true;
        layerToReveal.SetActive(true);
    }
    
}