using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pizza : MonoBehaviour
{
    [SerializeField] PizzaOrder order;
    public PizzaOrder Order { get { return order; } }

    [SerializeField] List<PizzaLayerReveal> layers;


    public List<PizzaIngredient> RevealedLayers {  get { List<PizzaIngredient> ingredients = new List<PizzaIngredient>(); 
            foreach(PizzaLayerReveal layer in layers.FindAll(l => l.Revealed).ToList()) ingredients.Add(layer.layersIngredient);
            return ingredients;
        } }

    internal void ActivateLayer(PizzaLayerReveal pizzaLayer)
    {
        if(layers.Exists(l => l.layersIngredient == pizzaLayer.layersIngredient))
            layers.Single(l => l.layersIngredient == pizzaLayer.layersIngredient).Reveal();

    }
}
