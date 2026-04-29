using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Pizza : MonoBehaviour
{
    [SerializeField] PizzaOrder order;
    public PizzaOrder Order { get { return order; } }

    [SerializeField] List<PizzaLayerReveal> layers;
    public UnityEvent OnStartCooking;
    public UnityEvent OnEndCooking;

    bool cooked = false;
    bool packed = false;

    public List<PizzaIngredient> RevealedLayers {  get { List<PizzaIngredient> ingredients = new List<PizzaIngredient>(); 
            foreach(PizzaLayerReveal layer in layers.FindAll(l => l.Revealed).ToList()) ingredients.Add(layer.layersIngredient);
            return ingredients;
        } 
    }

    internal void ActivateLayer(PizzaLayerReveal pizzaLayer)
    {
        if (!cooked && layers.Exists(l => l.layersIngredient == pizzaLayer.layersIngredient))
        {
            PizzaLayerReveal layer = layers.Find(l => l.layersIngredient == pizzaLayer.layersIngredient);
            if (!layer.Revealed)
                layer.Reveal();
        }
    }
    internal void ActivateLayer(PizzaToppingMono topping)
    {
        if (!cooked && layers.Exists(l => l.layersIngredient == topping.topping.toppingName))
        {
            PizzaLayerReveal layer = layers.Find(l => l.layersIngredient == topping.topping.toppingName);
            if (!layer.Revealed)
                layer.Reveal();
        }
    }
    //private void OnTriggerEnter(Collider collider)
    //{
    //    PizzaToppingMono ingredient;
    //    if (!collider.gameObject.TryGetComponent(out ingredient)) return;

    //    // בדוק אם זו התוספת הנכונה בתור
    //    if (!Order.IsNextTopping(ingredient.topping.toppingName))
    //    {
    //        return;
    //    }
    //    ActivateLayer(ingredient);
    //}
    //private void OnCollisionEnter(Collision collider)
    //{
    //    PizzaToppingMono ingredient;
    //    if (!collider.gameObject.TryGetComponent(out ingredient)) return;

    //    // בדוק אם זו התוספת הנכונה בתור
    //    if (!Order.IsNextTopping(ingredient.topping.toppingName))
    //    {
    //        return;
    //    }
    //    ActivateLayer(ingredient);
    //}

    public void InspectTopping(GameObject GO)
    {
        PizzaToppingMono ingredient;
        if (!GO.TryGetComponent(out ingredient)) return;

        // בדוק אם זו התוספת הנכונה בתור
        if (!Order.IsNextTopping(ingredient.topping.toppingName))
        {
            return;
        }
        ActivateLayer(ingredient);

    }
    public void StartCooking()
    {
        OnStartCooking.Invoke();
    }
    internal void EndCooking()
    {
        cooked = true;
        order.PizzaCooked();
        OnEndCooking.Invoke();
    }

    internal bool Pack(Transform packingLocation)
    {
        if (!cooked)
            return false;
        packed = true;
        GetComponent<ReturnToOriginAuto>().Override = true;
        transform.position = packingLocation.position;
        transform.rotation = packingLocation.rotation;
        order.PizzaDelivered();
        return true;
    }
}
