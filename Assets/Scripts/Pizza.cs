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
