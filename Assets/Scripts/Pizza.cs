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
    public bool Cooked { get => cooked; }
    bool packed = false;
    public bool Packed { get => packed; }

    public List<PizzaIngredient> RevealedLayers {  get { List<PizzaIngredient> ingredients = new List<PizzaIngredient>(); 
            foreach(PizzaLayerReveal layer in layers.FindAll(l => l.Revealed).ToList()) ingredients.Add(layer.layersIngredient);
            return ingredients;
        } 
    }

    public void Init(PizzaOrder order)
    {
        this.order = order;
    }
    internal void ActivateLayer(PizzaLayerReveal pizzaLayer)
    {
        Debug.Break();
        if (!cooked && layers.Exists(l => l.layersIngredient == pizzaLayer.layersIngredient))
        {
            PizzaLayerReveal layer = layers.Find(l => l.layersIngredient == pizzaLayer.layersIngredient);
            print("layer about to reveal: " + layer.layersIngredient +"\n is revealed: " + layer.Revealed);
            if (!layer.Revealed)
                layer.Reveal();
        }
    }
    internal void ActivateLayer(PizzaToppingMono topping)
    {
        if (!cooked && layers.Exists(l => l.layersIngredient == topping.topping.toppingName))
        {
            PizzaLayerReveal layer = layers.Find(l => l.layersIngredient == topping.topping.toppingName);
            print("layer about to reveal: " + layer.layersIngredient + "\n is revealed: " + layer.Revealed);
            if (!layer.Revealed)
                layer.Reveal();
        }
    }

    //bool PreviousGroupRevealed(PizzaIngredient ingredient)
    //{
    //    if(PizzaIngredientGroups.pizzaIngredientsGroups)
    //        return true;
    //    if(order.RequiredIngredients.)
    //}

    public void InspectTopping(GameObject GO)
    {
        PizzaToppingMono ingredient;
        if (!GO.TryGetComponent(out ingredient)) return;
        if(!ingredient.enabled) return;
        print("topping found in inspection");
        // בדוק אם זו התוספת הנכונה בתור
        //if (!Order.IsNextTopping(ingredient.topping.toppingName))
        //{
        //    return;
        //}
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
    public bool IsReadyForCooking()
    {
        return Order.RequiredIngredients.All(req => layers.Find(layers => layers.layersIngredient == req).Revealed);
    }

    public bool IsReadyForPacking()
    {
        return cooked;
    }
    internal void Pack(Transform packingLocation)
    {
        if (!cooked)
            return;
        packed = true;
        order.PizzaDelivered();
    }
}
