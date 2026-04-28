using UnityEngine;
using UnityEngine.Events;

public class PizzaLayerReveal : MonoBehaviour
{
    [SerializeField] Pizza pizza;
    public GameObject layerToReveal;
    public PizzaIngredient layersIngredient;
    [SerializeField] UnityEvent OnToppingAdded;
    
    PizzaToppingMono ingredient;
    bool revealed = false;
    public bool Revealed { get { return revealed; } }
    
    //void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.CompareTag("Ingredient"))
    //    {
    //        if(!other.gameObject.TryGetComponent(out ingredient)) return;
            
    //        // בדוק אם זו התוספת הנכונה בתור
    //        if (!pizza.Order.IsNextTopping(ingredient.topping.toppingName))
    //        {
    //            // לא בתור — חזרה למקום
    //            return;
    //        }

    //        // תוספת נכונה ובתור — חשוף את השכבה
    //        pizza.ActivateLayer(this);
    //        pizza.Order.ToppingAdded(ingredient.topping.toppingName);
    //        ingredient = null;
    //        //collision.gameObject.SetActive(false);

    //        // הודע לאונבורדינג
    //        //FindFirstObjectByType<OnboardingManager>()?.OnboardingActionCompleted(ingredient.toppingName.ToString());
    //        OnToppingAdded.Invoke();
    //    }
        
    //}
    public void Reveal()
    {
        if(revealed) return;
        print("Revealed");
        revealed = true;
        layerToReveal.SetActive(true);
    }
    
}